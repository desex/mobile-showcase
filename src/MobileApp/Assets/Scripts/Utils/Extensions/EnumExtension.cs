using System.ComponentModel;
using Sotbify.MobileApp.Utils.Enums;

namespace Sotbify.MobileApp.Utils.Extensions
{
    public static class EnumExtension
    {
        public static string ToStringExtension(this ENetworkStatus networkStatus)
        {
            return networkStatus switch
            {
                ENetworkStatus.Online => "Онлайн",
                ENetworkStatus.Offline => "Оффлайн",
                _ => throw new InvalidEnumArgumentException()
            };
        }
    }
}