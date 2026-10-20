using Sotbify.MobileApp.Utils.Enums.Blackjack;

namespace Sotbify.MobileApp.Utils.Extensions
{
    public static class CharExtension
    {
        public static ERank ToERank(this char @char)
        {
            return @char == '2' ? ERank.Two :
                @char == '3' ? ERank.Three :
                @char == '4' ? ERank.Four :
                @char == '5' ? ERank.Five :
                @char == '6' ? ERank.Six :
                @char == '7' ? ERank.Seven :
                @char == '8' ? ERank.Eight :
                @char == '9' ? ERank.Nine :
                @char == 'T' ? ERank.Ten :
                @char == 'J' ? ERank.Jack :
                @char == 'K' ? ERank.King :
                @char == 'Q' ? ERank.Queen :
                @char == 'A' ? ERank.A :
                ERank.None;
        }
        
        public static ESuit ToESuit(this char @char)
        {
            return @char == 'H' ? ESuit.Hearts :
                @char == 'D' ? ESuit.Diamonds :
                @char == 'C' ? ESuit.Clubs :
                @char == 'S' ? ESuit.Spades :
                ESuit.None;
        }
    }
}