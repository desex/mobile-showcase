using System;
using Sotbify.MobileApp.Utils.Enums;

namespace Sotbify.MobileApp.Utils.Data
{
    public sealed class NotificationData
    {
        public int Id;
        public string Title;
        public string Description;
        public ENotificationCategory Category;
        public long CreatedAt;
        public long SentAt;
        
        public DateTime CreatedAtDateTime => new(CreatedAt);
        public DateTime SentAtDateTime => new(SentAt);
    }
}