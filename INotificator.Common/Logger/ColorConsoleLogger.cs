using INotificator.Common.Models;
using System;
using Microsoft.Extensions.Logging;

namespace INotificator.Common.Logger
{

    public class ColorConsoleLogger : ILogger
    {
        private readonly string _name;
        private readonly ColorConsoleLoggerConfiguration _config;

        public ColorConsoleLogger(string name, ColorConsoleLoggerConfiguration config)
        {
            _name = name;
            _config = config;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
            Exception exception, Func<TState, Exception, string> formatter)
        {
            //if (_config.EventId == 0 || _config.EventId == eventId.Id)
            {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = logLevel switch
                {
                    LogLevel.Information => ConsoleColor.Yellow,
                    LogLevel.Debug => ConsoleColor.Green,
                    LogLevel.Warning => ConsoleColor.Magenta,
                    LogLevel.Error => ConsoleColor.Red,
                    LogLevel.Critical => ConsoleColor.DarkRed,
                    _ => color
                };
                Console.Write($"[{logLevel}]");
                Console.ForegroundColor = color;
                Console.WriteLine($"[{_name}] | {DateTime.Now.ToString("HH:mm:ss")} | {formatter(state, exception)}");
            }
        }
    }
}