using Sotbify.MobileApp.Utils.Enums;
using Sotbify.MobileApp.Utils.Enums.Blackjack;
using Sotbify.MobileApp.Utils.Enums.FeedContent;
using Sotbify.MobileApp.Utils.Enums.Home;
using Sotbify.MobileApp.Utils.Enums.Nexus;

namespace Sotbify.MobileApp.Services.ServerConstConverter
{
    public interface IServerConstConverter
    {
        public EChatSender ToEChatSender(string chatSender);
        public EMessageContentType ToEMessageContentType(string contentType);
        public EHomeItemType ToEHomeItemType(string contentType);
        public ENexusCategory ToENexusCategory(string contentType);
        public EBonusType ToEBonusType(string contentType);
        public EBlackjackRoundResult ToEBlackjackRoundResult(string blackjackResult);
        public EGameResult ToEResult(string result);
        public ENotificationCategory ToENotificationCategory(string category);
        public EDailyBonusType ToEDailyBonusType(string schemaType);
        public EFeedContentType ToEFeedContentType(string contentType);
    }
}