using System.Threading.Tasks;
using INotificator.Common.Models;

namespace INotificator.Common.Interfaces
{
    public interface IReceiver
    {
        /// <summary>
        /// Метод загружает данные
        /// </summary>
        /// <returns></returns>
        Task<DataResult<string>> GetData(string path);
    }
}