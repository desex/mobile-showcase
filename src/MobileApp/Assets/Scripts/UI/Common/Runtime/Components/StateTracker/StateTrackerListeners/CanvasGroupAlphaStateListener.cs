using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using Sirenix.OdinInspector;
using Sotbify.MobileApp.Utils;
using Sotbify.MobileApp.Utils.Extensions;
using UnityEngine;

namespace Sotbify.MobileApp.UI.Common.Components.StateTracker.StateTrackerListeners
{
    public sealed class CanvasGroupAlphaStateListener : StateTrackerAsyncListener<float>
    {
        [Required]
        [SerializeField] private CanvasGroup _canvasGroup;
        
        [SerializeField] private MotionParameters _motionParameters;
        
        private CancellationTokenSource _cancellationTokenSource;
        
        protected override async UniTask ChangeStateAsyncInternal(StateTracker.State state, float value, CancellationToken cancellationToken)
        {
            if (_canvasGroup == null)
                return;
            
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                _canvasGroup.alpha = value;
                return;
            }
#endif

            CreateToken(cancellationToken);
            
            var changeAlphaTask = _canvasGroup.CreateOpacityTask(
                _canvasGroup.alpha, value,
                _motionParameters, CancelBehavior.Complete,
                _cancellationTokenSource.Token
            );

            await changeAlphaTask;
        }

        private void CreateToken(CancellationToken cancellationToken)
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        }
    }
}