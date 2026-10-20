using System;
using Sotbifay.Analytics;
using Sotbifay.MobileApp.Logger;

namespace Sotbify.MobileApp.Analytics
{
    public sealed class AnalyticsLogger : IAnalyticsLogger
    {
        private readonly ILogger _logger;

        public AnalyticsLogger(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool IsEnabled(int logLevel) => 
            _logger.IsEnabled((LogLevel)logLevel);

        public void Log(int logLevel, string message, Exception exception, object context) => 
            _logger.Log((LogLevel)logLevel, message, exception, context);
    }
}