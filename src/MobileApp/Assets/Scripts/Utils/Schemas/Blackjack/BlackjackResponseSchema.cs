using System;
using Newtonsoft.Json;

namespace Sotbify.MobileApp.Utils.Schemas.Blackjack
{
    [Serializable]
    public class BlackjackResponseSchema
    {
        [JsonProperty("cards_left")] public int CardsLeft;
        [JsonProperty("player_cards")] public string[] PlayerCards;
        [JsonProperty("dealer_cards")] public string[] DealerCards;
        [JsonProperty("player_score")] public int PlayerScore;
        [JsonProperty("dealer_score")] public int? DealerScore;
        [JsonProperty("rounds_played")] public int RoundsPlayed;
        [JsonProperty("round_result")] public string RoundResult;
        [JsonProperty("game_result")] public string GameResult;
    }
}