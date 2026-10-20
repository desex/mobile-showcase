using System;

namespace Sotbify.MobileApp.Analytics
{
    public sealed partial class AnalyticsEvents
    {
        public void CopyBonus(string partner, string bonus)
        {
            if (string.IsNullOrEmpty(partner))
                throw new ArgumentException("Cannot be null or empty", nameof(partner));
            
            if (string.IsNullOrEmpty(bonus))
                throw new ArgumentException("Cannot be null or empty", nameof(bonus));
            
            using (GetBuilder(out var builder))
            {
                    builder.WithName("copy_bonus")
                    .WithParameter("partner", partner)
                    .WithParameter("bonus", bonus)
                    .WithParameter("notificationUUID", NotificationUUID)
                    .WithDestination(Destination)
                    .Build()
                    .Send();
            }
            
            Destination = AnalyticsDestination.AppMetrica;
        }
    }
}