using System.Collections.Generic;
using System.Threading.Tasks;
using INotificator.Common.Models;
using INotificator.Common.Models.Services.BaseProductService;

namespace INotificator.Common.Interfaces
{
    public interface IParser
    {
        /// <summary>
        /// Метод парсит результат и возвращает набор продуктов
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        DataResult<ICollection<Product>> ParseResult(string data);
    }
}