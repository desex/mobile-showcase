using System;
using Sotbifay.Analytics;

namespace Sotbify.MobileApp.Analytics
{
    public sealed class CreationTimeCommonParameter : IAnalyticsCommonParameter
    {
        public string Name => "creation_time";
        public bool CanBeCached => false;

        public object GetValue() => 
            DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}