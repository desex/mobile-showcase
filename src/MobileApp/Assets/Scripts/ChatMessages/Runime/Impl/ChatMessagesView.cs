using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.Common.OSA;
using Sotbify.MobileApp.UI.SafeArea;
using Sotbify.MobileApp.UI.Window.View;
using UnityEngine;
using VContainer;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.ChatMessages
{
    public sealed class ChatMessagesView : FadeWindowView
    {
        [SerializeField] private MessagesListView _messagesListView;
        [SerializeField] private int _additionalContentPaddingBottom;
        [SerializeField] private float _scrolledToEndTolerance = 10f;

        private ILogger _logger;

        private ISoftwareKeyboardArea _softwareKeyboardArea;
        private int _initialContentPaddingBottom;
        private float _previousKeyboardHeight;

        public MessagesListView MessagesListView => _messagesListView;

        public int AdditionalContentPaddingBottom
        {
            get => _additionalContentPaddingBottom;
            set
            {
                _additionalContentPaddingBottom = value;
                UpdateContentPaddingBottom(true);
            }
        }

        [Inject]
        public void Constructor(ILogger logger)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void TryScrollToBottom(int lastItemIndex)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override void OnAwakeInternal()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override void OnDestroyInternal()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override async UniTask ShowInternalAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void Update()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        private void UpdateContentPaddingBottom(bool force = false, bool scrollToBottom = true)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}