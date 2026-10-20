using System;

namespace Sotbifay.MobileApp.Logger
{
    public interface ILogger
    {
        LogLevel MinLevel { get; set; }

        void Log(LogLevel level, string message, Exception exception = null, object context = null);
    }
}