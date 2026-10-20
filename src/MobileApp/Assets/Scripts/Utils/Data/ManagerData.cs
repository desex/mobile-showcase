using System;
using Sotbify.MobileApp.Utils.Enums;

namespace Sotbify.MobileApp.Utils.Data
{
    public class ManagerData
    {
        public int Id;
        public string Username;
        public string Fullname;
        public string ThumbnailUrl;
        public long OnlineDate;
        public bool IsOnline;
        
        public DateTime OnlineDateTime => new(OnlineDate);

        public ENetworkStatus OnlineStatus => IsOnline
            ? ENetworkStatus.Online
            : OnlineDateTime - DateTime.UtcNow < TimeSpan.FromMinutes(5)
                ? ENetworkStatus.Online
                : ENetworkStatus.Offline;
    }
}