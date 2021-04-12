using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using INotificator.Common.Interfaces;
using INotificator.Common.Interfaces.Parsers;
using INotificator.Common.Interfaces.Receivers;
using INotificator.Common.Interfaces.Services;
using INotificator.Common.Models;
using INotificator.Common.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Options = INotificator.Common.Models.Options;

namespace INotificator.Services
{
    public class AvitoService : BaseProductService, IAvitoService
    {
        private const int UpdateDelayMinutes = 60;

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

        public Task SearchProducts()
        {
            if (_options.Avito?.Watchers == null)
            {
                return Task.CompletedTask;
            }

            if ((DateTime.Now - _lastStarted).TotalMinutes < UpdateDelayMinutes)
            {
                _logger.LogDebug($"Skipped. Will started after {UpdateDelayMinutes - (DateTime.Now - _lastStarted).TotalMinutes} min");
                return Task.CompletedTask;
            }
            _lastStarted = DateTime.Now;

            var tasks = new List<Task>();
            // ReSharper disable once PossibleNullReferenceException
            foreach (var watcher in _options.Avito?.Watchers?.Where(s => s.IsEnabled))
            {
                tasks.Add(base.SearchProducts(_receiver, _parser, $"{_options.Avito.Url}{watcher.Path}"));
            }

            return Task.WhenAll(tasks);
        }
    }
}