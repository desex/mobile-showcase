using System;
using System.Collections.Generic;

namespace Sotbifay.MobileApp.Logger
{
    public sealed class Logger : ILogger
    {
        private readonly IReadOnlyList<ILogSink> _logSinks;

        public Logger(IReadOnlyList<ILogSink> logSinks)
        {
            _logSinks = logSinks ?? throw new ArgumentNullException(nameof(logSinks));
        }

        public LogLevel MinLevel { get; set; }
        
        [UnityEngine.HideInCallstack]
        public void Log(LogLevel level, string message, Exception exception = null, object context = null)
        {
            if ((int)level < (int)MinLevel)
                return;
            
            foreach (var loggerDestination in _logSinks)
                loggerDestination.Log(level, message, exception, context);
        }
    }
}