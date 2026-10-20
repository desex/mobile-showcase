using System;

namespace Sotbify.MobileApp.Analytics
{
    public sealed partial class AnalyticsEvents
    {
        public void NotificationPermissionRequest(string permissionState)
        {
            if (string.IsNullOrEmpty(permissionState))
                throw new ArgumentException("Cannot be null or empty", nameof(permissionState));
            
            using (GetBuilder(out var builder))
            {
                builder.WithName("notification_access_request")
                    .WithParameter("status", permissionState)
                    .WithDestination(Destination)
                    .Build()
                    .Send();
            }
        }
        
        public void InAppPushReceived()
        {
            using (GetBuilder(out var builder))
            {
                builder.WithName("inapp_push_shown")
                    .WithDestination(Destination)
                    .Build()
                    .Send();
            }
        }

        public void InAppPushOpened(double timeToTap)
        {
            using (GetBuilder(out var builder))
            {
                builder.WithName("inapp_push_opened")
                    .WithParameter("time_to_tap_sec", timeToTap)
                    .WithDestination(Destination)
                    .Build()
                    .Send();
            }
        }

        public void InAppPushDismiss()
        {
            using (GetBuilder(out var builder))
            {
                builder.WithName("push_dismissed")
                    .WithDestination(Destination)
                    .Build()
                    .Send();
            }
        }

        public void InitialNotificationPermission(string permissionState)
        {
            if (string.IsNullOrEmpty(permissionState))
                throw new ArgumentException("Cannot be null or empty", nameof(permissionState));
            
            using (GetBuilder(out var builder))
            {
                builder.WithName("initial_notification_access")
                    .WithParameter("status", permissionState)
                    .WithDestination(Destination)
                    .Build()
                    .Send();
            }
        }
    }
}
