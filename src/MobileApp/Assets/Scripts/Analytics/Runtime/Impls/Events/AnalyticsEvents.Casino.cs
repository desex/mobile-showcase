using System;

namespace Sotbify.MobileApp.Analytics
{
    public sealed partial class AnalyticsEvents
    {
        public void PlayCasino(string partner)
        {
            if (string.IsNullOrEmpty(partner))
                throw new ArgumentException("Cannot be null or empty", nameof(partner));
            
            using (GetBuilder(out var builder))
            {
                builder.WithName("play_casino")
                    .WithParameter("partner", partner)
                    .WithParameter("notificationUUID", NotificationUUID)
                    .WithDestination(Destination)
                    .Build()
                    .Send();
            }
            
            Destination = AnalyticsDestination.AppMetrica;
        }
    }
}