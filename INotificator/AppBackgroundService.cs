using System;
using System.Threading;
using System.Threading.Tasks;
using INotificator.Common.Interfaces;
using INotificator.Common.Interfaces.Services;
using INotificator.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace INotificator
{
    public class AppBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IBackgroundService _service;
        
        public AppBackgroundService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            _service = scope.ServiceProvider.GetRequiredService<IBackgroundService>();
            await _service.StartAsync();
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _service.StopAsync();
            return base.StopAsync(cancellationToken);
        }
    }
}