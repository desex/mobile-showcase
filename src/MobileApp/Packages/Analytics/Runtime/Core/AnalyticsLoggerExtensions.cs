using System;
using System.Runtime.CompilerServices;

namespace Sotbifay.Analytics
{
    public static class AnalyticsLoggerExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEnabledVerbose(this IAnalyticsLogger logger) => 
            logger?.IsEnabled(0) ?? false;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEnabledDebug(this IAnalyticsLogger logger) => 
            logger?.IsEnabled(1) ?? false;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEnabledInformation(this IAnalyticsLogger logger) => 
            logger?.IsEnabled(2) ?? false;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEnabledWarning(this IAnalyticsLogger logger) => 
            logger?.IsEnabled(3) ?? false;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEnabledError(this IAnalyticsLogger logger) => 
            logger?.IsEnabled(4) ?? false;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEnabledFatal(this IAnalyticsLogger logger) => 
            logger?.IsEnabled(5) ?? false;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Verbose(this IAnalyticsLogger logger, string message, Exception exception = null, object context = null) =>
            logger?.Log(0, message, exception, context);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Debug(this IAnalyticsLogger logger, string message, Exception exception = null, object context = null) =>
            logger?.Log(1, message, exception, context);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Information(this IAnalyticsLogger logger, string message, Exception exception = null, object context = null) =>
            logger?.Log(2, message, exception, context);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Warning(this IAnalyticsLogger logger, string message, Exception exception = null, object context = null) =>
            logger?.Log(3, message, exception, context);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Error(this IAnalyticsLogger logger, string message, Exception exception = null, object context = null) =>
            logger?.Log(4, message, exception, context);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Fatal(this IAnalyticsLogger logger, string message, Exception exception = null, object context = null) =>
            logger?.Log(5, message, exception, context);
    }
}