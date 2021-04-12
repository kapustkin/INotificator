using System;
using System.Threading;
using System.Threading.Tasks;
using INotificator.Common.Interfaces.Services;
using INotificator.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace INotificator
{
    public class NotificationService : IHostedService
    {
        private readonly IDnsService _dnsService;
        private readonly IAvitoService _avitoService;
        private readonly IOnlinetradeService _onlinetradeService;
        private readonly ILogger _logger;

        private bool _isWorking = true;
        
        public NotificationService(
            IDnsService dnsService,
            IAvitoService avitoService,
            IOnlinetradeService onlinetradeService,
            ILogger<NotificationService> logger)
        {
            _dnsService = dnsService;
            _avitoService = avitoService;
            _onlinetradeService = onlinetradeService;
            _logger = logger;
        }        
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
#pragma warning disable 4014
            Start();
#pragma warning restore 4014
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _isWorking = false;
            
            return Task.CompletedTask;
        }

        private async Task Start()
        {
            while (_isWorking)
            {
                try
                {
                    var dns = _dnsService.SearchProducts();
                    var avito = _avitoService.SearchProducts();
                    var onlinetrade = _onlinetradeService.SearchProducts();

                    Task.WaitAll( dns, avito, onlinetrade );
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Unhandled error in service! {ex.Message} {ex.StackTrace}");
                }

                await Task.Delay(TimeSpan.FromMinutes(5));
            }
        }
    }
}