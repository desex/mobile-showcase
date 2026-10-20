using System;

namespace Sotbifay.MobileApp.Logger
{
    public interface ILogSink
    {
        void Log(LogLevel level, string message, Exception exception = null, object context = null);
    }
}