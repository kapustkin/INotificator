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
    public class OnlinetradeService : BaseProductService, IOnlinetradeService
    {
        private readonly IOnlinetradeReceiver _receiver;
        private readonly IOnlinetradeParser _parser;
        private readonly Options _options;
        public OnlinetradeService(
            IOnlinetradeReceiver receiver,
            IOnlinetradeParser parser,
            ISender sender,
            IStorage<Product> storage,
            ILogger<OnlinetradeService> logger,
            IOptions<Options> options) : base(sender, storage, logger)
        {
            _receiver = receiver;
            _parser = parser;
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public Task SearchProducts()
        {
            if (_options.Onlinetrade?.Watchers == null)
            {
                return Task.CompletedTask;
            }

            var tasks = new List<Task>();
            // ReSharper disable once PossibleNullReferenceException
            foreach (var watcher in _options.Onlinetrade?.Watchers?.Where(s => s.IsEnabled))
            {
                tasks.Add(base.SearchProducts(_receiver, _parser, $"{_options.Onlinetrade.Url}{watcher.Path}"));
            }

            return Task.WhenAll(tasks);
        }
    }
}