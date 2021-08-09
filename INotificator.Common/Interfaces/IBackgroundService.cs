using System.Threading.Tasks;

namespace INotificator.Common.Interfaces
{
    public interface IBackgroundService
    {
        Task StartAsync();
        
        Task StopAsync();
    }
}