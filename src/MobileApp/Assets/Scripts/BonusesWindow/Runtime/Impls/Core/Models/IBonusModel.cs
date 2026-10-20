using System;
using Sotbify.MobileApp.Utils.Enums;
using UnityEngine;

namespace Sotbify.MobileApp.BonusesWindow.Core.Models
{
    public interface IBonusModel
    {
        public Type CachedType { get; }
        public EBonusType ItemType { get; }
        public Vector2 CachedTypeSize { get; set; }
        public float CachedHeight { get; set; }
    }
}