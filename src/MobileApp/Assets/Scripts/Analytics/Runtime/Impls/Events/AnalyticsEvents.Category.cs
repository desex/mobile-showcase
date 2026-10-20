using System;

namespace Sotbify.MobileApp.Analytics
{
    public sealed partial class AnalyticsEvents
    {
        public void OpenCategory(string categoryEndpoint, Guid guid)
        {
            if (string.IsNullOrEmpty(categoryEndpoint))
                throw new ArgumentException("Cannot be null or empty", nameof(categoryEndpoint));
            
            using (GetBuilder(out var builder))
            {
                builder.WithName($"open_{categoryEndpoint}")
                    .WithParameter("guid", guid)
                    .WithDestination(Destination)
                    .Build()
                    .Send();
            }
        }
        
        public void CloseCategory(string categoryEndpoint, Guid guid)
        {
            if (string.IsNullOrEmpty(categoryEndpoint))
                throw new ArgumentException("Cannot be null or empty", nameof(categoryEndpoint));
            
            using (GetBuilder(out var builder))
            {
                builder.WithName($"close_{categoryEndpoint}")
                    .WithParameter("guid", guid)
                    .WithParameter("notificationUUID", NotificationUUID)
                    .WithDestination(Destination)
                    .Build()
                    .Send();
            }
            
            Destination = AnalyticsDestination.AppMetrica;
        }
    }
}