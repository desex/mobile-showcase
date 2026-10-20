namespace Sotbify.MobileApp.Db.ChatParameters
{
    public interface IChatParameters
    {
        public int PreLoadDifference { get; }
        public int DateShowTimeSpan { get; }
        public float KeyboardTopPadding { get; }
        public float ManagerStatusRequestDelayMinutes { get; }
    }
}