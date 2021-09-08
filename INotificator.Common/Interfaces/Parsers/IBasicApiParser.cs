using System.Collections;
using System.Collections.Generic;
using INotificator.Common.Models;

namespace INotificator.Common.Interfaces.Parsers
{
    public interface IBasicApiParser
    {
        /// <summary>
        /// Метод парсит результат и возвращает набор продуктов
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        DataResult<IEnumerable<T>> ParseResult<T>(string data);
    }
}