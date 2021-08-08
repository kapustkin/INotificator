using System.Collections;
using System.Collections.Generic;
using INotificator.Common.Models;

namespace INotificator.Common.Interfaces.Parsers
{
    public interface ILogToApiParser
    {
        /// <summary>
        /// Метод парсит результат и возвращает набор продуктов
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        DataResult<IEnumerable<LogRecord>> ParseResult(string data);
    }
}