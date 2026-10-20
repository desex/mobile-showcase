using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using R3;
using Sotbify.MobileApp.Utils;
using Sotbify.MobileApp.Utils.Extensions;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace Sotbify.MobileApp.NexusWindow.Dock
{
    public class DockItem : MonoBehaviour
    {
        [SerializeField] private RectTransform _iconRectTransform;
        [SerializeField] private Graphic _text;
        [SerializeField] private float _riseAmount;
        
        [SerializeField] private Button _button;

        [SerializeField] private MotionParameters _transactionMotionParameters;

        public bool IsSelected { get; private set; } = false;

        public Observable<Unit> ClickObservable => _button.OnClickAsObservable();

        public UniTask ActivateAsync(CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        public UniTask DeactivateAsync(CancellationToken cancellationToken = default) 
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private async UniTask ChangeVisibilityInternal(
            bool isVisible,
            CancellationToken cancellationToken
        )
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}