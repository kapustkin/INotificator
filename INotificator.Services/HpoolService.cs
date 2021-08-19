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
    /// <summary>
    /// Сервис проверки логов Hpool
    /// </summary>
    public class HpoolService : IHpoolService
    {
        private const int UpdateDelayMinutes = 30;
        private DateTime _lastStarted;
        
        private readonly ILogToApiReceiver _receiver;
        private readonly ILogToApiParser _parser;
        private readonly ISender _sender;
        
        private readonly ILogger _logger;
        private readonly Options _options;
        
        public HpoolService(
            ILogToApiReceiver receiver,
            ILogToApiParser parser,
            ISender sender,
            ILogger<HpoolService> logger,
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
            if (_options.Hpool.IsEnabled == false)
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
                _logger.LogDebug($"Hpool check space started {_options.Hpool.Url}");
                var rawData = await _receiver.GetData(_options.Hpool.Url);
                if (rawData.HasError)
                {
                    _logger.Log(LogLevel.Error, rawData.ErrorMessage);
                    return;
                }

                var logs = _parser.ParseResult(rawData.Data);

                var lastRecord = logs.Data.LastOrDefault(s => s.Message.Equals("new mining info"));
                if (lastRecord?.Capacity.Equals(_options.Hpool.TargetCapacity) == false)
                {
                    _logger.LogWarning(
                        $"Warning! Current capacity is '{lastRecord?.Capacity}'. Target capacity {_options.Hpool.TargetCapacity}");
                    await _sender.Send(new Message()
                    {
                        Source = "Hpool",
                        MessageText =
                            $"Внимание! Изменилось место Hpool на {lastRecord?.Capacity}. Ожидаемое значение {_options.Hpool.TargetCapacity}"
                    });
                }

                if (lastRecord != null && (DateTime.Now - lastRecord.DateTime).Minutes > 30)
                {
                    _logger.Log(LogLevel.Warning, $"No data more than 30 minutes");
                    await _sender.Send(new Message()
                    {
                        Source = "Hpool",
                        MessageText = "Внимание! Отсутвуют данные более 30 минут!"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"Возникло исключение при обратке ответа Hpool {ex.Message}");
            } 
            _logger.LogDebug($"Hpool check space end {_options.Hpool.Url}");
        }
    }
}