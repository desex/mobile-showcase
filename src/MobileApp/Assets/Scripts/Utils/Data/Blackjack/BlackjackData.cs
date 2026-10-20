using System.Collections.Generic;
using Sotbify.MobileApp.Utils.Enums.Blackjack;

namespace Sotbify.MobileApp.Utils.Data.Blackjack
{
    public sealed class BlackjackData
    {
        public int CardsLeft;
        public List<Card> PlayerCards;
        public List<Card> DealerCards;
        public int PlayerScore;
        public int DealerScore;
        public int RoundsPlayed;
        public EBlackjackRoundResult RoundResult;
        public EGameResult GameResult;
    }
}