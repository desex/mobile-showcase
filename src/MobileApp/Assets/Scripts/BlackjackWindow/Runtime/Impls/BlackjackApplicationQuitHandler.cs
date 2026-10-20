using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.Common.Preferences;
using Sotbify.MobileApp.UI.Window;
using UnityEngine;
using VContainer.Unity;

namespace Sotbify.MobileApp.BlackjackWindow
{
    public sealed class BlackjackApplicationQuitHandler : 
        IAsyncStartable, 
        IDisposable
    {
        private readonly IBlackjack _blackjack;
        private readonly IPreferences _preferences;
        private readonly IWindowService _windowService;
        private GameObject _lifecycleGameObject;
        private bool _cancelRequested;
        
        private const string ShouldCancelBlackjack = "IsBlackjackCancelled";

        public BlackjackApplicationQuitHandler(
            IBlackjack blackjack,
            IPreferences preferences,
            IWindowService windowService
        )
        {
            _blackjack = blackjack;
            _preferences = preferences;
            _windowService = windowService;
        }

        public async UniTask StartAsync(CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void OnApplicationQuitting()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        internal void MarkCancelOnApplicationExit()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void CreateLifecycleBehaviour()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void Dispose()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private sealed class BlackjackApplicationLifecycleBehaviour : MonoBehaviour
        {
            private BlackjackApplicationQuitHandler _handler;

            public void Construct(BlackjackApplicationQuitHandler handler)
            {
                // Здесь была реализация, не могу показать более
                throw new global::System.NotImplementedException();
            }

            private void OnApplicationPause(bool pauseStatus)
            {
                // Здесь была реализация, не могу показать более
                throw new global::System.NotImplementedException();
            }

            private void OnApplicationQuit()
            {
                // Здесь была реализация, не могу показать более
                throw new global::System.NotImplementedException();
            }
        }
    }
}