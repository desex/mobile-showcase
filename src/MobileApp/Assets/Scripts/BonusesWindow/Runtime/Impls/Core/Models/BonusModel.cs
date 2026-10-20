using System;
using System.Collections.Generic;
using System.Globalization;
using Sotbify.MobileApp.Utils.Enums;
using UnityEngine;

namespace Sotbify.MobileApp.BonusesWindow.Core.Models
{
    public class BonusModel : IBonusModel, IComparable<BonusModel>
    {
        private DateTime _createdAt;
        private DateTime _expiredAt;
        
        public BonusModel()
        {
            CachedType = GetType();
        }
        
        public int Id { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public IReadOnlyList<BonusLine> Lines { get; set; }
        public EBonusType ItemType { get; set; }
        public string Brand { get; set; }
        public Uri ActivationUri { get; set; }
        public Uri ImageUri { get; set; }
        public Uri RulesUri { get; set; }
        public bool IsOpened { get; set; }
        public Color BonusTypeColor { get; set; }
        public Type CachedType { get; }
        public Vector2 CachedTypeSize { get; set; }
        public float CachedHeight { get; set; }

        public string CreatedAt 
        {
            get => _createdAt.ToString("dd/MM/yy");
            set => DateTime.TryParse(value, null, DateTimeStyles.RoundtripKind, out _createdAt);
        }
        public string ExpiredAt 
        {
            get => _expiredAt.ToString("dd/MM/yy");
            set => DateTime.TryParse(value, null, DateTimeStyles.RoundtripKind, out _expiredAt);
        }

        public TimeSpan Duration => _expiredAt - DateTime.Now;

        public int CompareTo(BonusModel other)
        {
            return Duration.CompareTo(other.Duration);
        }
    }
}