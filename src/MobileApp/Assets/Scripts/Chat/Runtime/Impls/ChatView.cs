using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion.Animation;
using R3;
using Sotbify.MobileApp.UI.SafeArea;
using Sotbify.MobileApp.UI.Window.View;
using UnityEngine;

namespace Sotbify.MobileApp.Chat
{
    public sealed class ChatView : FadeWindowView
    {
        [SerializeField] private ManagerInfo _managerInfo;
        [SerializeField] private RectTransform _viewportRootTransform;
        [SerializeField] private GameObject _quickMessagesBar;
        [SerializeField] private RectTransform _messageInputRectTransform;

        [SerializeField] private LitMotionAnimation _loadingAnimation;

        private ISoftwareKeyboardArea _softwareKeyboardArea;
        private float _initialViewportBottomOffset;
        private float _previousKeyboardHeight;
        
        public ManagerInfo ManagerInfo => _managerInfo;
        
        public Vector2 MessageInputSize => _messageInputRectTransform != null 
            ? _messageInputRectTransform.sizeDelta
            : Vector2.zero;
        
        public bool LoadingAnimation
        {
            get => _loadingAnimation != null && _loadingAnimation.gameObject.activeSelf;
            set
            {
                if(_loadingAnimation != null)
                    _loadingAnimation.gameObject.SetActive(value);
            }
        }

        public void ChangeQuickMessagePanelVisibility(bool isVisible) {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override UniTask ShowInternalAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override UniTask HideInternalAsync(CancellationToken cancellationToken)
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

        private void Update()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}
