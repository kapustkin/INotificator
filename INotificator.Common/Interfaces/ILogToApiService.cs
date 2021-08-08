using System.Threading.Tasks;

namespace INotificator.Common.Interfaces
{
    public interface ILogToApiService
    {
        /// <summary>
        /// Метод проверки логов
        /// </summary>
        /// <returns></returns>
        Task CheckLog();
    }
}