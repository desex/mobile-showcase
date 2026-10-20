using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using LeTai.Asset.TranslucentImage;
using LitMotion;
using LitMotion.Extensions;
using R3;
using Sirenix.OdinInspector;
using Sotbify.MobileApp.UI.Window.View;
using Sotbify.MobileApp.Utils;
using Sotbify.MobileApp.Utils.Extensions;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace Sotbify.MobileApp.HomeWindow
{
    public class HomeView : FadeWindowView
    {
        private const string ActiveRelicSetGroup = "Active Relic Set";
        
        [SerializeField] private Button _cancelBlurButton;
        [SerializeField] private TranslucentImage _tranlucentImage;
        [SerializeField] private CanvasGroup _tranlucentImageCanvasGroup;
        [SerializeField] private MotionParameters _motionParameters;
        [SerializeField] private CanvasGroup _groupToHideWithBlur;
        [SerializeField] private RawImage _logoImage;

        [BoxGroup(ActiveRelicSetGroup)]
        [SerializeField] private List<ItemView> _items;
        [BoxGroup(ActiveRelicSetGroup)]
        [SerializeField] private Button _editActiveSetButton;
        [BoxGroup(ActiveRelicSetGroup)]
        [SerializeField] private Button _playButton;

        private bool _isBlurActive = true;

        public Observable<Unit> ChangeBlurStateButton => _cancelBlurButton.OnClickAsObservable();
        public TranslucentImageSource TranslucentImageSource
        {
            get => _tranlucentImage.source;
            set => _tranlucentImage.source = value;
        }

        public Texture Texture
        {
            get => _logoImage != null ? _logoImage.texture : null;
            set
            {
                if(_logoImage != null)
                    _logoImage.texture = value;
            }
        }
        
        public IReadOnlyList<ItemView> Items => _items;
        
        public Observable<Unit> EditActiveSetButton => _editActiveSetButton != null 
            ? _editActiveSetButton.OnClickAsObservable()
            : Observable.Empty<Unit>();
        
        public Observable<Unit> PlayObservable => _playButton != null 
            ? _playButton.OnClickAsObservable()
            : Observable.Empty<Unit>();

        public async UniTask ChangeBlurState(CancellationToken cancellationToken = default)
        {
            _isBlurActive = _isBlurActive == false;
            var targetAlpha = _isBlurActive ? 1 : 0;

            using (ListPool<UniTask>.Get(out var tasks))
            {
                var blurAnimation = LMotion.Create(_tranlucentImageCanvasGroup.alpha, targetAlpha, _motionParameters.Time)
                    .WithEase(_motionParameters.Ease)
                    .BindToAlpha(_tranlucentImageCanvasGroup)
                    .ToUniTask(cancellationToken);

                var changeVisibilityElements = _groupToHideWithBlur.CreateOpacityTask
                (
                    _groupToHideWithBlur.alpha, 
                    targetAlpha,
                    _motionParameters, 
                    CancelBehavior.Complete,
                    cancellationToken
                );

                _groupToHideWithBlur.blocksRaycasts = _isBlurActive;
                
                tasks.Add(changeVisibilityElements);
                tasks.Add(blurAnimation);
            
                await UniTask.WhenAll(tasks);
            }
        }
    }
}

