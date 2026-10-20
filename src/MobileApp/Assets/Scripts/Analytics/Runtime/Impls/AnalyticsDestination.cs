using System;

namespace Sotbify.MobileApp.Analytics
{
    [Flags]
    public enum AnalyticsDestination
    {
        None = 0,
        Everywhere = -1,
        
        AppMetrica = 1 << 1,
        Sotbifay = 1 << 2
    }
}