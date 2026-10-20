using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Sotbify.MobileApp.UI.Window.View
{
    public abstract class FadeWindowView : BaseWindowView
    {
        private const string AnimationGroup = "Animation";

        [BoxGroup(AnimationGroup)] 
        [SerializeField] private CanvasGroup _animationCanvasGroup;
        [BoxGroup(AnimationGroup)] 
        [SerializeField] private float _animationDuration = 0.33f;
        [BoxGroup(AnimationGroup)] 
        [SerializeField] private Ease _animationEase = Ease.InOutQuad;

        private bool _stateHasBeenChanged = false;
        private MotionHandle _motionHandle;

        private void Awake()
        {
            if (_animationCanvasGroup != null && _stateHasBeenChanged == false)
                _animationCanvasGroup.alpha = 0;

            OnAwakeInternal();
        }

        protected override async UniTask ShowInternalAsync(CancellationToken cancellationToken)
        {
            _stateHasBeenChanged = true;

            if (_animationCanvasGroup == null)
                return;

            try
            {
                _animationCanvasGroup.blocksRaycasts = false;

                if (_motionHandle.IsActive())
                    _motionHandle.Cancel();

                _motionHandle = GetAnimation(1f);
                await _motionHandle.ToUniTask(cancellationToken: cancellationToken);
            }
            finally
            {
                _animationCanvasGroup.blocksRaycasts = true;
            }
        }

        protected override async UniTask HideInternalAsync(CancellationToken cancellationToken)
        {
            _stateHasBeenChanged = true;

            if (_animationCanvasGroup == null)
                return;

            _animationCanvasGroup.blocksRaycasts = false;

            if (_motionHandle.IsActive())
                _motionHandle.Cancel();

            _motionHandle = GetAnimation(0f);
            await _motionHandle.ToUniTask(cancellationToken: cancellationToken);
        }

        private MotionHandle GetAnimation(float targetAlpha)
        {
            var currentAlpha = _animationCanvasGroup.alpha;
            return LMotion
                .Create(currentAlpha, targetAlpha, _animationDuration)
                .WithEase(_animationEase)
                .BindToAlpha(_animationCanvasGroup)
                .AddTo(_animationCanvasGroup);
        }

        protected virtual void OnAwakeInternal()
        {
        }
    }
}