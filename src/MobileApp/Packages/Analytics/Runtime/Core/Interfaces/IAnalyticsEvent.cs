using System;

namespace Sotbifay.Analytics
{
    public interface IAnalyticsEvent : IDisposable
    {
        void Send();
    }
}