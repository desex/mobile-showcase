using System;
using System.Text;
using Sotbify.MobileApp.Services.FirebaseCrashlyticsService;

namespace Sotbifay.MobileApp.Logger.Sinks.Runtime
{
    public class FirebaseCrashlyticsLogSink : ILogSink
    {
        private readonly IFirebaseCrashlyticsService _firebaseCrashlyticsService;

        public FirebaseCrashlyticsLogSink(IFirebaseCrashlyticsService firebaseCrashlyticsService)
        {
            _firebaseCrashlyticsService = firebaseCrashlyticsService;
        }
        
        public void Log(LogLevel level, string message, Exception exception = null, object context = null)
        {
            if (_firebaseCrashlyticsService.IsInitialized == false)
                return;
            
            if (string.IsNullOrWhiteSpace(message) == false)
            {
                var stringBuilder = new StringBuilder();
                if (context is not null)
                {
                    stringBuilder.Append('[')
                        .Append(context.GetType().Name)
                        .Append("] ");
                }

                stringBuilder.Append("Log level: ")
                    .Append(level)
                    .Append("; ")
                    .Append("Message: ")
                    .Append(message);
                
                var targetMessage = stringBuilder.ToString(); 
            
                _firebaseCrashlyticsService.Log(targetMessage);

                if ((level == LogLevel.Error || level == LogLevel.Fatal) && exception is null)
                    exception = new Exception(targetMessage);
            }
            
            if (exception is not null)
                _firebaseCrashlyticsService.LogException(exception);
        }
    }
}