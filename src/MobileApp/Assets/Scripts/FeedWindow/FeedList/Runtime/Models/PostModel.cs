using System;
using System.Globalization;
using Sotbify.MobileApp.Utils.Enums;
using UnityEngine;

namespace Sotbify.MobileApp.FeedWindow.FeedList.Models
{
    public class PostModel : IFeedModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Uri ImageUri { get; set; }
        public Color? Color { get; set; }
        
        public bool ShowHrefButtons { get; set; }
        public string ButtonText { get; set; }
        public string ButtonHref { get; set; }
        public bool IsRead { get; set; }
        public EHomeItemType ItemType { get; set; }

        public string PublishedAt
        {
            set
            {
                DateTime.TryParse(value, null, DateTimeStyles.RoundtripKind, out var utcTime);
                
                utcTime = DateTime.SpecifyKind(utcTime, DateTimeKind.Utc);
                
                PublishedAtDateTime = utcTime.ToLocalTime();
            }
        }

        public Type CachedType { get; }
        public Vector2 CachedTypeSize { get; set; }
        public Vector2 CachedStatusSize { get; set; }
        public float CachedSelfHeight { get; set; }
        public float CachedHeightForWidth { get; set; }
        public DateTime PublishedAtDateTime { get; private set; }

        public PostModel()
        {
            CachedType = GetType();
        }
    }
}
