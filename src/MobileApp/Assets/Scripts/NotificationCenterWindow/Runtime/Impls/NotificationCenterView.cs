using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion.Animation;
using R3;
using Sotbify.MobileApp.Localization;
using Sotbify.MobileApp.UI.Window.View;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.UI;

namespace Sotbify.MobileApp.NotificationCenterWindow
{
    public sealed class NotificationCenterView : FadeWindowView
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private LocalizationVariableEntry<IntVariable> _unreadCountLocalizationEntry;
        [SerializeField] private LitMotionAnimation _loadingCircle;
        [SerializeField] private NotificationsListView _notificationsListView;
        
        public NotificationsListView NotificationsListView =>_notificationsListView;
        public Observable<Unit> CloseObservable => _closeButton.OnClickAsObservable();

        public int UnreadCount
        {
            get => _unreadCountLocalizationEntry.GetValueOrDefault();
            set => _unreadCountLocalizationEntry.TrySetValue(value);
        }

        public void LoadingAnimation(bool shouldShow)
            {
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
    }
}