﻿using System;
using System.Linq;
using System.Threading.Tasks;
using INotificator.Common.Interfaces;
using INotificator.Common.Models;
using INotificator.Common.Services;
using Microsoft.Extensions.Logging;

namespace INotificator.Services
{
    public abstract class BaseProductService
    {
        private readonly ISender _sender;
        private readonly IStorage<Product> _storage;
        private readonly ILogger _logger;

        protected BaseProductService(
            ISender sender,
            IStorage<Product> storage,
            ILogger logger)
        {
            _sender = sender;
            _storage = storage;
            _logger = logger;
        }

        protected async Task SearchProducts(IReceiver receiver, IParser parser, string url)
        {
            _logger.LogDebug($"Search products started {url}");

            var data = await receiver.GetData(url);
            if (data.HasError)
            {
                await _sender.Send(new Message {MessageText = $"Receiver error! {data.ErrorMessage}"});
                return;
            }

            var products = parser.ParseResult(data.Data);
            if (products.HasError)
            {
                await _sender.Send(new Message {MessageText = $"Parser error! {data.ErrorMessage}"});
                return;
            }

            //Проверяем есть ли новые продукты
            if (products?.Data?.Any() ?? false)
            {
                foreach (var product in products.Data.Take(10).Reverse())
                {
                    if (_storage.GetItem(product.Name) == null)
                    {
                        var sended = false;
                        var count = 0;
                        while (!sended)
                        {
                            try
                            {
                                sended = await _sender.Send(
                                    new Message
                                    {
                                        Source = product.Source,
                                        MessageText = product.Price != null
                                            ? $"{product.Name} | {product.Price}р"
                                            : $"{product.Name}",
                                        Link = product.Url,
                                    });
                                count++;
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

                        var success = await _storage.AddItem(product);
                        if (!success)
                        {
                            _logger.LogError("Error write data to storage");
                        }
                    }
                }
            }

            _logger.LogDebug($"Search products finished");
        }
    }
}