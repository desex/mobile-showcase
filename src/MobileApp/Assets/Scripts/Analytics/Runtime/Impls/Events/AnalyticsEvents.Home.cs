using System;

namespace Sotbify.MobileApp.Analytics
{
    public sealed partial class AnalyticsEvents
    {
        public void OpenNews(int newsId, Guid guid)
        {
            using (GetBuilder(out var builder))
            {
                builder.WithName("open_news")
                    .WithParameter("entry_id", guid)
                    .WithParameter("news_id", newsId.ToString())
                    .WithParameter("notificationUUID", NotificationUUID)
                    .WithDestination(Destination)
                    .Build()
                    .Send();
            }

            Destination = AnalyticsDestination.AppMetrica;
        }
        
        public void CloseNews(Guid guid)
        {
            using (GetBuilder(out var builder))
            {
                builder.WithName("close_news")
                    .WithParameter("entry_id", guid)
                    .WithDestination(Destination)
                    .Build()
                    .Send();
            }
        }
    }
}