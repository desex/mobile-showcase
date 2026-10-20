using System;
using Sotbifay.Analytics;

namespace Sotbify.MobileApp.Analytics
{
    public sealed class ApplicationStateCommonParameter : IAnalyticsCommonParameter
    {
        public string Name => "application_state";
        public bool CanBeCached => false;
        public Type State { get; set; } = null;
        
        public object GetValue() => 
            State?.Name ?? "<empty>";
    }
}