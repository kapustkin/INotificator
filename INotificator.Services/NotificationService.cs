using System;
using System.Threading;
using System.Threading.Tasks;
using INotificator.Common.Interfaces;
using INotificator.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace INotificator.Services
{
    /// <summary>
    /// Главный сервис
    /// </summary>
    public class NotificationService : IBackgroundService
    {
        private readonly IDnsService _dnsService;
        private readonly IAvitoService _avitoService;
        private readonly IOnlinetradeService _onlinetradeService;
        private readonly IHpoolService _hpoolService;
        private readonly IComputerUniverseService _computerUniverseService;
        private readonly ILogger _logger;

        private bool _isWorking = true;

        private readonly CancellationTokenSource _token = new();
        
        public NotificationService(
            IDnsService dnsService,
            IAvitoService avitoService,
            IOnlinetradeService onlinetradeService,
            IHpoolService hpoolService,
            IComputerUniverseService computerUniverseService,
            ILogger<NotificationService> logger)
        {
            _dnsService = dnsService;
            _avitoService = avitoService;
            _onlinetradeService = onlinetradeService;
            _hpoolService = hpoolService;
            _computerUniverseService = computerUniverseService;
            _logger = logger;
        }        
        
        public async Task StartAsync()
        {
            while (_isWorking)
            {
                try
                {
                    var dns = _dnsService.SearchProducts();
                    var avito = _avitoService.SearchProducts();
                    var onlinetrade = _onlinetradeService.SearchProducts();
                    var computerUniverse = _computerUniverseService.SearchProducts();

                    var hpoolService = _hpoolService.CheckLog();
                    
                    await Task.WhenAll( dns, avito, onlinetrade, computerUniverse, hpoolService);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Unhandled error in service! {ex.Message} {ex.StackTrace}");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), _token.Token);
            }
            
            _logger.Log(LogLevel.Debug, "Background service is shutdown");
        }

        public Task StopAsync()
        {
            _isWorking = false;
            _token.Cancel();
            _token.Dispose();
            
            return Task.CompletedTask;
        }
    }
}