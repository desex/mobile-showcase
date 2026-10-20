using System;
using System.Runtime.CompilerServices;

namespace Sotbifay.MobileApp.Logger
{
    public static class LoggerExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEnabled(this ILogger logger, LogLevel level)
        {
            TryThrowArgumentNullException(logger);
            return ((int)level) >= ((int)logger.MinLevel);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.HideInCallstack]
        public static void Verbose(this ILogger logger, string message, Exception exception = null, object context = null)
        {
            TryThrowArgumentNullException(logger);
            logger.Log(LogLevel.Verbose, message, exception, context);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.HideInCallstack]
        public static void Debug(this ILogger logger, string message, Exception exception = null, object context = null)
        {
            TryThrowArgumentNullException(logger);
            logger.Log(LogLevel.Debug, message, exception, context);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.HideInCallstack]
        public static void Information(this ILogger logger, string message, Exception exception = null, object context = null)
        {
            TryThrowArgumentNullException(logger);
            logger.Log(LogLevel.Information, message, exception, context);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.HideInCallstack]
        public static void Warning(this ILogger logger, string message, Exception exception = null, object context = null)
        {
            TryThrowArgumentNullException(logger);
            logger.Log(LogLevel.Warning, message, exception, context);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.HideInCallstack]
        public static void Error(this ILogger logger, string message, Exception exception = null, object context = null)
        {
            TryThrowArgumentNullException(logger);
            logger.Log(LogLevel.Error, message, exception, context);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.HideInCallstack]
        public static void Fatal(this ILogger logger, string message, Exception exception = null, object context = null)
        {
            TryThrowArgumentNullException(logger);
            logger.Log(LogLevel.Fatal, message, exception, context);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void TryThrowArgumentNullException(ILogger logger)
        {
            if(logger is null)
                throw new ArgumentNullException(nameof(logger));
        }
    }
}