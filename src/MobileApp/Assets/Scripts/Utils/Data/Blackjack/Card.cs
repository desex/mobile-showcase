using System;
using Sotbify.MobileApp.Utils.Enums.Blackjack;

namespace Sotbify.MobileApp.Utils.Data.Blackjack
{
    public class Card : IEquatable<Card>
    {
        public ERank Rank;
        public ESuit Suit;
        public bool IsHidden = false;


        public bool Equals(Card other)
        {
            if (other is null) 
                return false;
            
            if (ReferenceEquals(this, other)) 
                return true;
            
            return Rank == other.Rank && Suit == other.Suit;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) 
                return false;
            
            if (ReferenceEquals(this, obj)) 
                return true;
            
            if (obj.GetType() != GetType()) 
                return false;
            
            return Equals((Card)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)Rank, (int)Suit, IsHidden);
        }
    }
}