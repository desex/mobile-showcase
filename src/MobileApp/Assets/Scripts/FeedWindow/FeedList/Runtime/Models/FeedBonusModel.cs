using System;
using Sotbify.MobileApp.Utils.Enums;
using UnityEngine;

namespace Sotbify.MobileApp.FeedWindow.FeedList.Models
{
    public class FeedBonusModel : IFeedModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Promocode { get; set; }
        public EBonusType BonusType { get; set; }
        public Uri BonusImageUri { get; set; }
        public string ActivationUrl { get; set; }
        public EHomeItemType ItemType { get; set; }

        public Type CachedType { get; }
        public Vector2 CachedTypeSize { get; set; }
        public Vector2 CachedStatusSize { get; set; }

        public FeedBonusModel()
        {
            CachedType = GetType();
        }
    }
}