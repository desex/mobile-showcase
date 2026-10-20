namespace Sotbify.MobileApp.Db.NotificationWindowConfig
{
    public interface INotificationWindowConfig
    {
        public int PreloadAmount { get; }
        public CategoryToSprite CategoryToSprite { get; }
    }
}