using Koba.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koba.Logging
{
    public class BotLogger : ILogger
    {
        private readonly string categoryName;
        private readonly StreamWriter logFile;

        public BotLogger(string categoryName, StreamWriter logFile) {
            this.categoryName = categoryName;
            this.logFile = logFile;
        }

        public IDisposable BeginScope<TState>(TState state) {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel) {
            return logLevel >= LogLevel.Information;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) {
            if(!IsEnabled(logLevel)) {
                throw new InvalidOperationException("LogLevel não habilitado.");
            }

            string message = formatter(state, exception);

            logFile.WriteLine($"[{TimeOnly.FromDateTime(DateTime.Now)}] [{logLevel}] [{categoryName}] {message}");
            logFile.Flush();
        }
    }
}
