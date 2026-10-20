using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion.Animation;
using R3;
using Sotbify.MobileApp.NewsWindow.Core;
using Sotbify.MobileApp.UI.Common.Components;
using Sotbify.MobileApp.UI.Window.View;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.NewsWindow
{
    public class NewsContentWindow : FadeWindowView
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private Image _colorImage;
        [SerializeField] private SpriteWithLoader _image;
        [SerializeField] private LitMotionAnimation _loadingAnimation;
        [SerializeField] private RectTransform _maskRectTransform;
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _previousButton;
        [SerializeField] private Button _closeWindowButton;
        [SerializeField] private LoadingOverlay _loadingOverlay;
        
        [field: SerializeField] public NewsContentListView NewsContentList { get; private set; }

        public string Title { set => _title.text = value; }
        public Sprite Image { set => _image.Sprite = value; }
        public Observable<Unit> NextPressed => _nextButton.OnClickAsObservable();
        public Observable<Unit> CloseObservable => _closeWindowButton.OnClickAsObservable();
        public Observable<Unit> PreviousPressed => _previousButton.OnClickAsObservable();
        public bool CanLoadNext { set => _nextButton.interactable = value; }
        public bool CanLoadPrevious { set => _previousButton.interactable = value; }
        
        public void StartLoadingAnimation()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void StopLoadingAnimation()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void ShowLoading() {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public UniTask HideLoading(CancellationToken cancellationToken) {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void ChangeProgress(float currentProgress)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        protected override UniTask ShowInternalAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}
