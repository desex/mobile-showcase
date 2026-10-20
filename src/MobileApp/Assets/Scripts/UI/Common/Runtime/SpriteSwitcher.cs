using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Sotbify.MobileApp.UI.Common
{
    public sealed class SpriteSwitcher : UIBehaviour
    {
        [Required]
        [SerializeField] private Image _image;
        [SerializeField] private bool _randomDelay = true;
        [ShowIf(nameof(_randomDelay))]
        [SerializeField] private Vector2 _delayRange = Vector2.right;
        [HideIf(nameof(_randomDelay))]
        [SerializeField] private float _frameDelay = 0.33333f;
        [SerializeField] private bool _randomSprite = true;
        [SerializeField] private List<Sprite> _sprites = new();

        private CancellationTokenSource _isActiveCancellationTokenSource;
        
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            _image ??= GetComponent<Image>();
        }
#endif

        protected override void OnEnable()
        {
            Reset();
        }

        private void Reset()
        {
            _image.sprite = _sprites[0];
        }

        public void Play()
        {
            _isActiveCancellationTokenSource?.Cancel();
            _isActiveCancellationTokenSource?.Dispose();
            _isActiveCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
            
            FrameSwitchAsync(_isActiveCancellationTokenSource.Token)
                .Forget();
        }

        public void Stop()
        {
            Reset();
            _isActiveCancellationTokenSource?.Cancel();
            _isActiveCancellationTokenSource?.Dispose();
            _isActiveCancellationTokenSource = null;
        }

        private async UniTaskVoid FrameSwitchAsync(CancellationToken cancellationToken)
        {
            try
            {
                var spriteIndex = -1;
                while (cancellationToken.IsCancellationRequested == false)
                {
                    if (_randomSprite == false)
                    {
                        spriteIndex = ++spriteIndex % _sprites.Count;
                    }
                    else
                    {
                        spriteIndex = Random.Range(0, _sprites.Count);
                    }
                
                    _image.sprite = _sprites[spriteIndex];
                
                    var delay = _randomDelay
                        ? TimeSpan.FromSeconds(Random.Range(_delayRange.x, _delayRange.y))
                        : TimeSpan.FromSeconds(_frameDelay);
                
                    await UniTask.Delay(delay, cancellationToken: cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}