using System;
using System.Threading.Tasks;
using INotificator.Common.Interfaces;
using INotificator.Common.Interfaces.Parsers;
using INotificator.Common.Interfaces.Receivers;
using INotificator.Common.Interfaces.Services;
using INotificator.Common.Models;
using INotificator.Common.Models.Services.BaseProductService;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Options = INotificator.Common.Models.Options;

namespace INotificator.Services
{
    public class DnsService : BaseProductService, IDnsService
    {
        private readonly IDnsReceiver _receiver;
        private readonly IDnsParser _parser;
        private readonly Options _options;
        public DnsService(
            IDnsReceiver receiver,
            IDnsParser parser,
            ISender sender,
            IStorage<Product> storage,
            ILogger<DnsService> logger,
            IOptions<Options> options) : base(sender, storage, logger)
        {
            _receiver = receiver;
            _parser = parser;
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public Task SearchProducts()
        {
            if (_options.Dns?.IsEnabled == false)
            {
                return Task.CompletedTask;
            }

            return base.SearchProducts(_receiver, _parser, $"{_options.Dns.Url}{_options.Dns.Configurator}", false);
        }
    }

 
}