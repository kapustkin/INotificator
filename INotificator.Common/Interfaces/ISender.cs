using System.Threading.Tasks;
using INotificator.Common.Models;

namespace INotificator.Common.Services
{
    public interface ISender
    {
        Task<bool> Send(Message messages);
    }
}