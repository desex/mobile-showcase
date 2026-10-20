using Sotbify.MobileApp.Utils.Enums.Blackjack;

namespace Sotbify.MobileApp.Utils.Extensions.Blackjack
{
    public static class ERankExtensions
    {
        public static string ToDisplayString(this ERank rank)
        {
            return rank switch
            {
                ERank.A => "A",
                ERank.Jack => "J",
                ERank.Queen => "Q",
                ERank.King => "K",
                ERank.Two or ERank.Three or ERank.Four or ERank.Five or ERank.Six or ERank.Seven or ERank.Eight or ERank.Nine or ERank.Ten
                    => ((int)rank).ToString(),
                _ => rank.ToString()
            };
        }

        public static int ToInt(this ERank rank)
        {
            return rank switch
            {
                ERank.Jack => 10,
                ERank.Queen => 10,
                ERank.King => 10,
                _ => (int)rank
            };
        }
    }
}