using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbifay.MobileApp.Common.CounterCircle
{
    public class Circle : MonoBehaviour
    {
        [SerializeField] private Image _circleImage;
        [SerializeField] private LayoutElement _selfLayout;
        [SerializeField] private float _increaseWidthDelta = 16f;
        [SerializeField] private Color _targetColor;
        [SerializeField] private Color _initialColor;

        public UniTask Activate(Ease ease, float duration, CancellationToken cancellationToken = default)
        {
            var initialPreferredWidth = _selfLayout.preferredWidth;
            var targetPreferredWidth = _selfLayout.preferredWidth + _increaseWidthDelta;
            
            var increaseTask = LMotion
                .Create(initialPreferredWidth, targetPreferredWidth, duration)
                .WithEase(ease)
                .Bind(value => _selfLayout.preferredWidth = value)
                .AddTo(_selfLayout)
                .ToUniTask(CancelBehavior.Cancel, cancellationToken);
            
            var colorChangeTask = LMotion
                .Create(_initialColor, _targetColor, duration)
                .WithEase(ease)
                .BindToColor(_circleImage)
                .AddTo(_circleImage)
                .ToUniTask(CancelBehavior.Cancel, cancellationToken);
            
            return UniTask.WhenAll(increaseTask,colorChangeTask);
        }

        public UniTask Deactivate(Ease ease, float duration, CancellationToken cancellationToken = default)
        {
            var initialPreferredWidth = _selfLayout.preferredWidth;
            var targetPreferredWidth = _selfLayout.preferredWidth - _increaseWidthDelta;

            var decreaseTask = LMotion
                .Create(initialPreferredWidth, targetPreferredWidth, duration)
                .WithEase(ease)
                .Bind(value => _selfLayout.preferredWidth = value)
                .AddTo(_selfLayout)
                .ToUniTask(CancelBehavior.Cancel, cancellationToken);
            
            var colorChangeTask = LMotion
                .Create(_targetColor, _initialColor, duration)
                .WithEase(ease)
                .BindToColor(_circleImage)
                .AddTo(_circleImage)
                .ToUniTask(CancelBehavior.Cancel, cancellationToken);
            
            return UniTask.WhenAll(decreaseTask,colorChangeTask);
        }
    }
}