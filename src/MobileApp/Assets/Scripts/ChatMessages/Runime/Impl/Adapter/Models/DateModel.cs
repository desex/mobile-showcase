using System;
using UnityEngine;

namespace Sotbify.MobileApp.ChatMessages.Models
{
    public class DateModel : IChatItemModel
    {
        public int ID { get; set; }
        public bool FreezeEndEdge { get; set; }
        public bool KeepVelocityOnCountChange { get; set; }
        public Vector2 CachedSize { get; set; }
        public Type CachedType { get; }
        public DateTime CreatedAt { get; set; }

        public DateModel()
        {
            CachedType = GetType();
        }
    }
}