using System.Threading;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using Sotbify.MobileApp.Common.OSA;
using Sotbify.MobileApp.UI.Window.Presenter;
using UnityEngine.Pool;

namespace Sotbify.MobileApp.BonusesWindow
{
    public class BonusesPresenter : WindowPresenter<IBonusesWindow, BonusesWindowView>
    {
        private readonly CompositeDisposable _disposables = new();

        public override void Initialize()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override async UniTask AfterShowAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        protected override UniTask AfterHideAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override void OnDispose()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        private void SubscribeModels<T>(IReadOnlyObservableList<T> list, ListDataHelper<T> targetList)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}