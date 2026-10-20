using System.IO;
using Sotbify.MobileApp.Services.RemoteConfig;
using Sotbify.MobileApp.Utils.Enums;
using Sotbify.MobileApp.Utils.Enums.Blackjack;
using Sotbify.MobileApp.Utils.Enums.FeedContent;
using Sotbify.MobileApp.Utils.Enums.Home;
using Sotbify.MobileApp.Utils.Enums.Nexus;

namespace Sotbify.MobileApp.Services.ServerConstConverter
{
    public class ServerConstConverter : IServerConstConverter
    {
        private readonly IRemoteConfigService _remoteConfigService;

        public ServerConstConverter(IRemoteConfigService remoteConfigService)
        {
            _remoteConfigService = remoteConfigService;
        }
        
        public EChatSender ToEChatSender(string chatSender)
        {
            return Equals(chatSender,"ManagerChatSender") ? EChatSender.Manager
                : Equals(chatSender,"UserMobileChatSender") || 
                  Equals(chatSender,"UserVRChatSender") ? EChatSender.User
                : throw new InvalidDataException();
        }

        public EMessageContentType ToEMessageContentType(string contentType)
        {
            return Equals(contentType, "ChatTextContent") ? EMessageContentType.Text
                : Equals(contentType,"ChatImageContent") ? EMessageContentType.Image
                : Equals(contentType,"ChatBlackjackContent") ? EMessageContentType.Blackjack
                : Equals(contentType,"ChatBlackjackResultContent") ? EMessageContentType.BlackjackResult
                : EMessageContentType.None;
        }

        public EHomeItemType ToEHomeItemType(string contentType)
        {
            return Equals(contentType, "HomeItem_News") ? EHomeItemType.ProjectNews
                : Equals(contentType, "HomeItem_AppNews") ? EHomeItemType.AppNews
                : Equals(contentType, "HomeItem_Bonus") ? EHomeItemType.Bonus
                : EHomeItemType.None;
        }

        public ENexusCategory ToENexusCategory(string contentType)
        {
            return Equals(contentType,"DockCategory_Home") ? ENexusCategory.Home
                : Equals(contentType,"DockCategory_Chat") ? ENexusCategory.Chat
                : Equals(contentType,"DockCategory_Tournaments") ? ENexusCategory.Feed
                : Equals(contentType,"DockCategory_Bonus") ? ENexusCategory.Bonuses
                : ENexusCategory.None;
        }

        public EBonusType ToEBonusType(string contentType)
        {
            return Equals(contentType,"BonusType_FreeSpins") ? EBonusType.FreeSpins
                : Equals(contentType,"BonusType_Money") ? EBonusType.Money
                : Equals(contentType,"BonusType_FreeBet") ? EBonusType.FreeBet
                : Equals(contentType,"BonusType_VipBonus") ? EBonusType.VipBonus
                : Equals(contentType,"BonusType_Deposit") ? EBonusType.Deposit
                : Equals(contentType,"BonusType_NoDeposit") ? EBonusType.NoDeposit
                : Equals(contentType,"BonusType_Personal") ?  EBonusType.Personal
                : EBonusType.None;
        }
        
        public EBlackjackRoundResult ToEBlackjackRoundResult(string blackjackResult)
        {
            return Equals(blackjackResult,"BlackjackRoundResult_Win") ? EBlackjackRoundResult.Win
                : Equals(blackjackResult,"BlackjackRoundResult_Blackjack")  ? EBlackjackRoundResult.Win
                : Equals(blackjackResult,"BlackjackRoundResult_Draw")  ? EBlackjackRoundResult.Tie
                : Equals(blackjackResult,"BlackjackRoundResult_Bust")  ? EBlackjackRoundResult.Lose
                : Equals(blackjackResult,"BlackjackRoundResult_Lose")  ? EBlackjackRoundResult.Lose
                : EBlackjackRoundResult.None;
        }

        public EGameResult ToEResult(string result)
        {
            return Equals(result,"BlackjackGameResult_Win")  ? EGameResult.Win
                : Equals(result,"BlackjackGameResult_Lose")  ? EGameResult.Lost
                : Equals(result,"BlackjackGameResult_Canceled")  ? EGameResult.Cancelled
                : EGameResult.None;
        }
        
        public EFeedContentType ToEFeedContentType(string contentType)
        {
            return Equals(contentType,"NewsContentText")  ? EFeedContentType.Text
                : Equals(contentType,"NewsContentImage")  ? EFeedContentType.Image
                : EFeedContentType.None;
        }

        public ENotificationCategory ToENotificationCategory(string category)
        {
            return Equals(category,"NotificationCategory_News")  ? ENotificationCategory.News
                : Equals(category,"NotificationCategory_Bonus") ? ENotificationCategory.Bonus
                : Equals(category,"NotificationCategory_PersonalBonus") ? ENotificationCategory.PersonalBonus
                : Equals(category,"NotificationCategory_System") ? ENotificationCategory.System
                : Equals(category,"NotificationCategory_Tournament") ? ENotificationCategory.Tournament
                : Equals(category,"NotificationCategory_Slot") ? ENotificationCategory.Slot
                : ENotificationCategory.None;
        }

        public EDailyBonusType ToEDailyBonusType(string type)
        {
            return Equals(type,"DailyBonus_Promocode")  ? EDailyBonusType.Promocode
                : EDailyBonusType.None;
        }

        private bool Equals(string value, string configKey)
            => value?.Equals(_remoteConfigService.GetString(configKey)) ?? false;
    }
}