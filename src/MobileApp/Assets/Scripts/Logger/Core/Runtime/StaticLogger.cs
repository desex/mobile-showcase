using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Sotbifay.MobileApp.Logger
{
    public static class StaticLogger
    {
        public static ILogger Logger;

        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Verbose(string message, Exception exception = null, object context = null)
        {
            Logger?.Log(LogLevel.Verbose, message, exception, context);
        }

        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Debug(string message, Exception exception = null, object context = null)
        {
            Logger?.Log(LogLevel.Debug, message, exception, context);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Information(string message, Exception exception = null, object context = null)
        {
            Logger?.Log(LogLevel.Information, message, exception, context);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Warning(string message, Exception exception = null, object context = null)
        {
            Logger?.Log(LogLevel.Warning, message, exception, context);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Error(string message, Exception exception = null, object context = null)
        {
            Logger?.Log(LogLevel.Error, message, exception, context);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Fatal(string message, Exception exception = null, object context = null)
        {
            Logger?.Log(LogLevel.Fatal, message, exception, context);
        }
    }
}