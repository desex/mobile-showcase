using System;
using R3;
using Sotbify.MobileApp.UI.Window.View;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.TutorialWindows.ChatNotificationTutorialOverlay
{
    public class ChatNotificationTutorialOverlayView : FadeWindowView
    {
        [SerializeField] private Button _enableNotificationsButton;
        [SerializeField] private Button _maybeLaterButton;
        
        public Observable<Unit> EnableNotificationsObservable => _enableNotificationsButton.OnClickAsObservable();
        public Observable<Unit> MaybeLaterObservable => _maybeLaterButton.OnClickAsObservable();
    }
}