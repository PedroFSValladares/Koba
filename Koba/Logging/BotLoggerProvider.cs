using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koba.Logging
{
    class BotLoggerProvider : ILoggerProvider
    {
        private readonly StreamWriter logFile;

        public BotLoggerProvider(StreamWriter logFile) {
            this.logFile = logFile;
        }
        public ILogger CreateLogger(string categoryName) {
            return new BotLogger(categoryName, logFile);
        }

        public void Dispose() {
            logFile.Flush();
            logFile.Close();
        }
    }
}
