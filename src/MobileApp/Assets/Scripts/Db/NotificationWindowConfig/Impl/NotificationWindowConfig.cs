using System;
using Sotbify.MobileApp.Common.SerializableDictionary;
using Sotbify.MobileApp.Utils.Enums;
using UnityEngine;

namespace Sotbify.MobileApp.Db.NotificationWindowConfig
{
    [CreateAssetMenu(menuName = "Settings/NotificationWindow_Config", fileName = "NotificationWindow_Config")]
    public class NotificationWindowConfig : ScriptableObject, INotificationWindowConfig
    {
        [field: SerializeField]
        public int PreloadAmount { get; private set; } = 10;

        [field: SerializeField]
        public CategoryToSprite CategoryToSprite { get; private set; }
    }
    
    [Serializable]
    public class CategoryToSprite : SerializableDictionary<ENotificationCategory, Sprite>
    {
    }
}