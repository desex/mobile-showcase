using System;
using System.Text;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Sotbifay.MobileApp.Logger.Sinks.Runtime
{
    public sealed class UnityLogSink : ILogSink
    {
        private readonly ObjectPool<StringBuilder> _stringBuilderPool = new(() => new StringBuilder(), actionOnRelease: sb => sb.Clear());
        private readonly UnityEngine.ILogger _unityLogger = Debug.unityLogger;

        [HideInCallstack]
        public void Log(LogLevel level, string message, Exception exception = null, object context = null)
        {
            var logType = Convert(level);
            var unityContext = context as Object;

            using (_stringBuilderPool.Get(out var stringBuilder))
            {
                if (context is not null)
                {
                    stringBuilder
#if UNITY_EDITOR
                        .Append("<b>")
#endif
                        .Append("[")
                        .Append(context.GetType().Name)
                        .Append("]: ")
#if UNITY_EDITOR
                        .Append("</b>")
#endif
                        ;
                }

                stringBuilder.Append(message);

                if (exception is not null)
                {
                    stringBuilder
                        .AppendLine()
#if UNITY_EDITOR
                        .Append("<b><color=#FF534A>")
#endif
                        .Append("Exception: ")
#if UNITY_EDITOR
                        .Append("</b></color>")
#endif
                        .Append(exception.Message);
                }

                if (unityContext != null)
                {
                    _unityLogger.Log(logType, message: stringBuilder.ToString(), context: unityContext);
                }
                else
                {
                    _unityLogger.Log(logType, message: stringBuilder.ToString());
                }

                if (exception is not null)
                    _unityLogger.LogException(exception, unityContext);
            }
        }

        private static LogType Convert(LogLevel level)
        {
            return level switch
            {
                LogLevel.Verbose => LogType.Log,
                LogLevel.Debug => LogType.Log,
                LogLevel.Information => LogType.Log,
                LogLevel.Warning => LogType.Warning,
                LogLevel.Error => LogType.Error,
                LogLevel.Fatal => LogType.Exception,
                _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
            };
        }
    }
}