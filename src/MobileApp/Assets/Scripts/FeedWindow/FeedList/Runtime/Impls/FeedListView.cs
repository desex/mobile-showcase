using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.Common.SerializableDictionary;
using Sotbify.MobileApp.FeedWindow.FeedList.Core;
using Sotbify.MobileApp.UI.Window.View;
using Sotbify.MobileApp.Utils.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.FeedWindow.FeedList
{
    public class FeedListView : FadeWindowView
    {
        [field: SerializeField] public FeedOSAView FeedOsaView { get; private set; }

        protected override UniTask ShowInternalAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
    
    [Serializable]
    public sealed class NavigationItems : SerializableDictionary<EHomeItemType, Button>
    {
    }
}

