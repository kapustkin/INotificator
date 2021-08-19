using System;
using System.Collections.Generic;
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
    public class AvitoService : BaseProductService, IAvitoService
    {
        private const int UpdateDelayMinutes = 30;
        private bool hasSendedErrorNotification = false;

        private DateTime _lastStarted;
        private readonly IAvitoReceiver _receiver;
        private readonly IAvitoParser _parser;
        private readonly ILogger _logger;
        private readonly Options _options;
        public AvitoService(
            IAvitoReceiver receiver,
            IAvitoParser parser,
            ISender sender,
            IStorage<Product> storage,
            ILogger<AvitoService> logger,
            IOptions<Options> options) : base(sender, storage, logger)
        {
            _receiver = receiver;
            _parser = parser;
            _logger = logger;
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task SearchProducts()
        {
            if (_options.Avito?.Watchers == null)
            {
                return;
            }

            if ((DateTime.Now - _lastStarted).TotalMinutes < UpdateDelayMinutes)
            {
                _logger.LogDebug($"Skipped. Will started after {(int)(UpdateDelayMinutes - (DateTime.Now - _lastStarted).TotalMinutes)} min");
                return;
            }

            // ReSharper disable once PossibleNullReferenceException
            var isSuccess = true;
            foreach (var watcher in _options.Avito?.Watchers?.Where(s => s.IsEnabled))
            {
                isSuccess &= await base.SearchProducts(_receiver, _parser, $"{_options.Avito.Url}{watcher.Path}",
                    watcher.DisableNotification);
            }
            
            if (isSuccess)
            {
                _lastStarted = DateTime.Now;
                hasSendedErrorNotification = false;
                return;
            }

            if ((DateTime.Now - _lastStarted).Minutes > UpdateDelayMinutes * 2 && hasSendedErrorNotification == false)
            {
                await SendMessage($"Внимание! Данные не обновлялись более {UpdateDelayMinutes * 2} минут");
                hasSendedErrorNotification = true;
            }
        }
    }
}