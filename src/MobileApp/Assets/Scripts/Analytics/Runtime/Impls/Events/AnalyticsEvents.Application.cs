using System;

namespace Sotbify.MobileApp.Analytics
{
    public sealed partial class AnalyticsEvents
    {
        public void ApplicationLaunch()
        {
            using (GetBuilder(out var builder))
            {
                builder.WithName("application_launch")
                    .WithDestination(Destination)
                    .Build()
                    .Send();
            }
        }
        
        public void EnterApplicationState(Guid entryId)
        {
            using (GetBuilder(out var builder))
            {
                builder.WithName("enter_application_state")
                    .WithDestination(Destination)
                    .WithParameter("entry_id", entryId)
                    .Build()
                    .Send();
            }
        }
        
        public void ExitApplicationState(Guid entryId)
        {
            using (GetBuilder(out var builder))
            {
                builder.WithName("exit_application_state")
                    .WithDestination(Destination)
                    .WithParameter("entry_id", entryId)
                    .WithParameter("notificationUUID", NotificationUUID)
                    .Build()
                    .Send();
            }
            
            Destination = AnalyticsDestination.AppMetrica;
        }
    }
}