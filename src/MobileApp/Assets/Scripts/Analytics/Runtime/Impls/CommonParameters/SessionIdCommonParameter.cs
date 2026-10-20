using System;
using Sotbifay.Analytics;

namespace Sotbify.MobileApp.Analytics
{
    public sealed class SessionIdCommonParameter : IAnalyticsCommonParameter
    {
        private readonly Guid _sessionId =  Guid.NewGuid();
        
        public string Name => "session_id";
        public bool CanBeCached => true;

        public object GetValue() => _sessionId;
    }
}