using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sotbify.MobileApp.UI.Window.View
{
    public abstract class BaseWindowView : MonoBehaviour, IWindowView
    {
        private CancellationTokenSource _visibilityChangeTokenSource;
        
        public async UniTask ShowAsync(CancellationToken cancellationToken = default)
        {
            _visibilityChangeTokenSource?.Cancel();
            _visibilityChangeTokenSource?.Dispose();
            
            _visibilityChangeTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, destroyCancellationToken);
            try
            {
                SetActiveInternal(true);
                await ShowInternalAsync(_visibilityChangeTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                SetActiveInternal(false);
            }
        }

        public async UniTask HideAsync(CancellationToken cancellationToken = default)
        {
            _visibilityChangeTokenSource?.Cancel();
            _visibilityChangeTokenSource?.Dispose();

            _visibilityChangeTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, destroyCancellationToken);
            try
            {
                await HideInternalAsync(_visibilityChangeTokenSource.Token);
                SetActiveInternal(false);
            }
            catch (OperationCanceledException)
            {
                SetActiveInternal(true);
            }
        }

        private void OnDestroy()
        {
            _visibilityChangeTokenSource?.Cancel();
            _visibilityChangeTokenSource?.Dispose();
            _visibilityChangeTokenSource = null;

            OnDestroyInternal();
        }

        protected virtual void SetActiveInternal(bool isActive) => 
            gameObject.SetActive(isActive);
        
        protected abstract UniTask ShowInternalAsync(CancellationToken cancellationToken);
        protected abstract UniTask HideInternalAsync(CancellationToken cancellationToken);
        
        protected virtual void OnDestroyInternal()
        {
        }
    }
}