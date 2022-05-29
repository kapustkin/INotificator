using System;
using System.Linq;
using System.Threading.Tasks;
using INotificator.Common.Interfaces;
using INotificator.Common.Interfaces.Parsers;
using INotificator.Common.Interfaces.Receivers;
using INotificator.Common.Interfaces.Services;
using INotificator.Common.Models.Services.Senders;
using INotificator.Common.Models.Services.ToMiners;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Options = INotificator.Common.Models.Options;

namespace INotificator.Services
{
    /// <summary>
    /// Сервис проверки воркеров 2Miners
    /// </summary>
    public class ToMinersService : IToMinersService
    {
        private const long Megahash = 1000000;
        private const int UpdateDelayMinutes = 30;
        private const string Source = "2Miners";

        private DateTime _lastStarted;

        /// <summary>
        /// Дата последней выплаты. При запуске считаем что текущее время
        /// </summary>
        private DateTime? _lastPayment = DateTime.Now;

        /// <summary>
        /// Кол-во воркеров
        /// </summary>
        private int? _workersOnline = null;

        private readonly IBasicApiReceiver _receiver;
        private readonly IBasicApiParser _parser;
        private readonly ISender _sender;

        private readonly ILogger _logger;
        private readonly Options _options;

        public ToMinersService(
            IBasicApiReceiver receiver,
            IBasicApiParser parser,
            ISender sender,
            ILogger<ToMinersService> logger,
            IOptions<Options> options)
        {
            _receiver = receiver;
            _parser = parser;
            _sender = sender;
            _logger = logger;
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task CheckLog()
        {
            if (_options.ToMiners.IsEnabled == false)
            {
                return;
            }

            if ((DateTime.Now - _lastStarted).TotalMinutes < UpdateDelayMinutes)
            {
                _logger.LogDebug($"Skipped. Will started after {(int)(UpdateDelayMinutes - (DateTime.Now - _lastStarted).TotalMinutes)} min");
                return;
            }
            _lastStarted = DateTime.Now;

            try
            {
                _logger.LogDebug($"2Miners check started {_options.ToMiners.Url}");
                var rawData = await _receiver.GetData(_options.ToMiners.Url);
                if (rawData.HasError)
                {
                    _logger.Log(LogLevel.Error, rawData.ErrorMessage);
                    return;
                }

                var response = _parser.ParseResult<ApiData>(rawData.Data);
                await CheckResponse(response.Data, _options.ToMiners);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"Возникло исключение при обработке ответа {ex.Message}");
            }
            _logger.LogDebug($"2Miners check end {_options.ToMiners.Url}");
        }

        /// <summary>
        /// Обработка ответа
        /// </summary>
        /// <param name="data"></param>
        /// <param name="config"></param>
        private async Task CheckResponse(ApiData data, Options.ToMinersConfig config)
        {
            if (data.ApiVersion != config.ApiVersion)
            {
                _logger.LogWarning(
                    $"Warning! Api version changed");
                await _sender.Send(new Message()
                {
                    Source = Source,
                    MessageText =
                        $"Внимание! Изменилась версия Api! Ожидаемая версия '{config.ApiVersion}', текущая '{data.ApiVersion }'"
                });

                return;
            }

            if (data.WorkersOnline != _workersOnline)
            {
                _workersOnline = data.WorkersOnline;
                _logger.LogWarning($"Warning! Workers count changed");
                await _sender.Send(new Message()
                {
                    Source = Source,
                    MessageText = $"Внимание! Изменилось количество воркеров. Текущее кол-во '{data.WorkersOnline}'. Хешрейт {data.Hashrate / Megahash:N1} MH/s"
                });
            }

            var payment = data.Payments?.FirstOrDefault();

            if (payment != null && _lastPayment < payment.Timestamp)
            {
                _lastPayment = payment.Timestamp;

                await _sender.Send(new Message()
                {
                    Source = Source,
                    MessageText =
                        $"Осуществлена выплата. Количество: {payment.Amount / (Megahash * 1000):0.######} ETH"
                });
            }
        }
    }
}