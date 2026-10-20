using Sotbifay.Analytics;
using UnityEngine;

namespace Sotbify.MobileApp.Analytics
{
    public sealed class ApplicationVersionCommonParameter : IAnalyticsCommonParameter
    {
        public string Name => "application_version";
        public bool CanBeCached => true;

        public object GetValue() => 
            Application.version;
    }
}