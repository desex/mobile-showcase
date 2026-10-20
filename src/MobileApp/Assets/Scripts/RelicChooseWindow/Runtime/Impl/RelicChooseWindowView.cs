using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.Common.OSA;
using Sotbify.MobileApp.Relic;
using Sotbify.MobileApp.RelicChooseWindow.Adapter;
using Sotbify.MobileApp.UI.Window.View;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.RelicChooseWindow
{
    public sealed class RelicChooseWindowView : FadeWindowView
    {
        [SerializeField] private RelicChooseListView _listView;
        [SerializeField] private RelicSmallDescriptionView _descriptionView;
        [SerializeField] private Button _closeButton;
        [SerializeField] private int _prewarmCount = 10;

        public RelicChooseListView ListView => _listView;
        public RelicSmallDescriptionView RelicSmallDescriptionView => _descriptionView;
        
        public Observable<(int Id, IRelicInstance RelicInstance)> OpenRelicDescriptionObservable => _listView.OpenDescriptionObservable;
        public Observable<Unit> CloseObservable => _closeButton.OnClickAsObservable();
        
        protected override UniTask ShowInternalAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}