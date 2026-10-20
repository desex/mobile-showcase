using System;
using UnityEngine;

namespace Sotbify.MobileApp.ChatMessages.Models
{
    public interface IChatItemModel
    {
        public int ID { get; }
        public bool FreezeEndEdge { get; set; }
        public bool KeepVelocityOnCountChange { get; set; }
        public Vector2 CachedSize { get; set; }
        public Type CachedType { get; }
        public DateTime CreatedAt { get; set; }
    }
}