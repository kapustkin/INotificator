using System;
using System.Linq;
using System.Threading.Tasks;
using INotificator.Common.Interfaces;
using INotificator.Common.Interfaces.Parsers;
using INotificator.Common.Interfaces.Receivers;
using INotificator.Common.Interfaces.Services;
using INotificator.Common.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Options = INotificator.Common.Models.Options;

namespace INotificator.Services
{
    public class ComputerUniverseService : IComputerUniverseService
    {
        private readonly IBasicApiReceiver _receiver;
        private readonly IBasicApiParser _parser;
        private readonly ISender _sender;
        private readonly IStorage<Product> _storage;
        private readonly ILogger _logger;
        private readonly Options _options;

        public ComputerUniverseService(
            IBasicApiReceiver receiver,
            IBasicApiParser parser,
            ISender sender,
            IStorage<Product> storage,
            ILogger<HpoolService> logger,
            IOptions<Options> options)
        {
            _receiver = receiver;
            _parser = parser;
            _sender = sender;
            _storage = storage;
            _logger = logger;
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task SearchProducts()
        {
            if (_options.ComputerUniverse.IsEnabled == false)
            {
                return;
            }

            // Проверяем с 9 утра до 19
            if (DateTime.Now.Hour is < 9 or >= 19)
            {
                return;
            }

            try
            {
                _logger.LogDebug($"ComputerUniverse checking started {_options.ComputerUniverse.Url}");
                var rawData = await _receiver.GetData(_options.ComputerUniverse.Url);
                if (rawData.HasError)
                {
                    _logger.Log(LogLevel.Error, rawData.ErrorMessage);
                    return;
                }

                var products = _parser.ParseResult<ComputerUniverseProduct>(rawData.Data);

                if (products?.Data?.Any() ?? false)
                {
                    foreach (var product in products.Data.Where(s =>
                        s.Status.Equals("В наличии и готов к отправке", StringComparison.InvariantCultureIgnoreCase)
                        && s.PriceEur is not (null or "")))
                    {
                        var storageProduct = await _storage.GetItem(product.Name);
                        if (storageProduct == null || !storageProduct.Price.Equals($"{product.PriceRur} / {product.PriceEur}", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var baseProduct = new Product()
                            {
                                Date = DateTime.Now,
                                Name = product.Name,
                                Price = $"{product.PriceRur} / {product.PriceEur}",
                                Url = product.Url,
                                Source = "ComputerUniverse"
                            };

                            if (!await SendMessage(baseProduct))
                            {
                                _logger.LogError("Error sending data"); 
                            }

                            var success = await _storage.AddItem(baseProduct);
                            if (!success)
                            {
                                _logger.LogError("Error write data to storage");
                            }
                        }
                    }
                }
                
                _logger.LogDebug($"ComputerUniverse checking products finished");
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"Возникло исключение при обратке ответа Hpool {ex.Message}");
            }
        }

        private async Task<bool> SendMessage(Product product)
        {
            var isSend = false;
            var count = 0;
            while (!isSend)
            {
                try
                {
                    isSend = await _sender.Send(
                        new Message
                        {
                            Source = product.Source,
                            MessageText = product.Price != null
                                ? $"{product.Name} | {product.Price}"
                                : $"{product.Name}",
                            Link = product.Url,
                        }, false);
                    count++;
                    if (isSend) return true;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Send message exception {ex.Message}");
                    //Ошибка отправки. Ждем и пробуем еще
                    if (count < 10)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(15));
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return false;
        }
    }
}