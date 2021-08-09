using System.Collections.Generic;
using System.Threading.Tasks;

namespace INotificator.Common.Interfaces
{
    public interface IStorage<T>
    {
        /// <summary>
        /// Get item from storage
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<T> GetItem(string name);

        /// <summary>
        /// Add item in storage
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<bool> AddItem(T item);

        /// <summary>
        /// Save collection in storage
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        Task<bool> SaveItems(ICollection<T> products);
    }
}