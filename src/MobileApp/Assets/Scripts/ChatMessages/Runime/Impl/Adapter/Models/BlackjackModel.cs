using System;
using Sotbify.MobileApp.Utils.Enums.Blackjack;
using UnityEngine;

namespace Sotbify.MobileApp.ChatMessages.Models
{
    public class BlackjackModel : IChatItemModel
    {
        public int ID { get; set; }
        public bool FreezeEndEdge { get; set; }
        public bool KeepVelocityOnCountChange { get; set; }
        public Vector2 CachedSize { get; set; }
        public EGameResult GameResult { get; set; } = EGameResult.None;
        public int RequestID { get; set; }
        public DateTime CreatedAt { get; set; }

        public Type CachedType { get; }

        public BlackjackModel()
        {
            CachedType = GetType();
        }
    }
}