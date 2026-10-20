using System;
using Io.AppMetrica;

namespace Sotbifay.MobileApp.Logger.Sinks.Runtime
{
    public sealed class AppMetricaLogSink : ILogSink
    {
        public LogLevel MinLevel { get; set; } = LogLevel.Error;

        public void Log(LogLevel level, string message, Exception exception = null, object context = null)
        {
            if (((int)level) < ((int)MinLevel))
                return;

            if (AppMetrica.IsActivated() == false)
                return;
            
            if (context is null)
            {
                if(message is not null && exception is not null)
                    AppMetrica.ReportError(message, exception);
            }
            else
            {
                AppMetrica.ReportError(context.GetType().Name, message, exception);
            }
        }
    }
}