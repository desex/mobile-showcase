using System;
using Com.ForbiddenByte.OSA.Core;
using R3;
using Sotbify.MobileApp.ChatMessages.Models;
using Sotbify.MobileApp.ChatMessages.ViewsHolders;
using Sotbify.MobileApp.Common.OSA;
using Sotbify.MobileApp.Utils.Enums.Blackjack;
using Sotbify.MobileApp.Utils.Extensions;
using UnityEngine;
using VContainer;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.ChatMessages
{
    public class MessagesListView : OSA<ChatParams, ChatItemViewsHolder>
    {
        private readonly Subject<BlackjackModel> _playBlackjackRequested = new(); 
        private readonly Subject<int> _lastVisibleItemUpdated = new();
        
        [SerializeField] private GameObject _isEmptyRoot;

        private ILogger _logger;
        private ListDataHelper<IChatItemModel> _data;

        public ListDataHelper<IChatItemModel> Data => _data ??= new ListDataHelper<IChatItemModel>(this, true);
        public Observable<int> LastVisibleItemUpdated => _lastVisibleItemUpdated;
        public Observable<BlackjackModel> PlayBlackjackRequested => _playBlackjackRequested;
        
        public int FirstVisibleItemIndex { get; private set; }
        public int LastVisibleItemIndex { get; private set; }

        public bool IsEmpty
        {
            get => _isEmptyRoot != null && _isEmptyRoot.activeSelf;
            set
            {
                if(_isEmptyRoot != null)
                    _isEmptyRoot.SetActive(value);
            }
        }

        [Inject]
        public void Construct(ILogger logger)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        public override void ScrollTo(int itemIndex, float normalizedOffsetFromViewportStart = 0, float normalizedPositionOfItemPivotToUse = 0)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public bool TryGetVisibleBlackjackViewsHolderByItemIndex(int itemIndex, out BlackjackViewsHolder blackjackViewsHolder)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void CancelActiveBlackjack()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override ChatItemViewsHolder CreateViewsHolder(int itemIndex)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        protected override float UpdateItemSizeOnTwinPass(ChatItemViewsHolder viewsHolder)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        protected override bool IsRecyclable(ChatItemViewsHolder potentiallyRecyclable, int indexOfItemThatWillBecomeVisible, double sizeOfItemThatWillBecomeVisible)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        protected override void UpdateViewsHolder(ChatItemViewsHolder holder)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        protected override void CollectItemsSizes(
            ItemCountChangeMode changeMode, 
            int count, 
            int indexIfInsertingOrRemoving, 
            ItemsDescriptor itemsDescriptor
        )
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override void OnScrollPositionChanged(double normalizedPosition)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override void OnDestroy()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}