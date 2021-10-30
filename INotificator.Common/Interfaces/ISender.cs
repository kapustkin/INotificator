using System.Threading.Tasks;
using INotificator.Common.Models;
using INotificator.Common.Models.Services.Senders;

namespace INotificator.Common.Interfaces
{
    public interface ISender
    {
        /// <summary>
        /// Метод отправляет сообщение с нотификацией
        /// </summary>
        /// <param name="messages">Сообщение</param>
        /// <returns></returns>
        Task<bool> Send(Message messages);
        
        /// <summary>
        /// Метод отправляет сообщение
        /// </summary>
        /// <param name="messages">Сообщение</param>
        /// <param name="disableNotification">Отключение нотификации</param>
        /// <returns></returns>
        Task<bool> Send(Message messages, bool disableNotification);
    }
}