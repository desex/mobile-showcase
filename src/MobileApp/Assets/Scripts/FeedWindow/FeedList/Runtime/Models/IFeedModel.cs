using System;
using Sotbify.MobileApp.Utils.Enums;
using UnityEngine;

namespace Sotbify.MobileApp.FeedWindow.FeedList.Models
{
    public interface IFeedModel
    {
        public Type CachedType { get; }
        public EHomeItemType ItemType { get; }
        public Vector2 CachedTypeSize { get; set; }
        public Vector2 CachedStatusSize { get; set; }
    }
}