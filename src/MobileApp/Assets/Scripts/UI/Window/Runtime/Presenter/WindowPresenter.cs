using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.UI.Window.View;

namespace Sotbify.MobileApp.UI.Window.Presenter
{
    public abstract class WindowPresenter<TWindowModel, TWindowView> : IWindowPresenter<TWindowModel, TWindowView>
        where TWindowModel : class
        where TWindowView : IWindowView
    {
        private readonly CancellationTokenSource _tokenSource = new();

        protected TWindowModel Model { get; private set; }
        protected TWindowView View { get; private set; }
        
        public virtual void Initialize()
        {
        }

        public void BindModel(object windowModel) => 
            BindModel((TWindowModel)windowModel);

        public void BindModel(TWindowModel windowModel)
        {
            Model = windowModel ?? throw new ArgumentNullException(nameof(windowModel));
            OnBindModel(Model);
        }

        public void BindView(IWindowView windowView) => 
            BindView((TWindowView)windowView);

        public void BindView(TWindowView windowView)
        {
            View = windowView ?? throw new ArgumentNullException(nameof(windowView));
        }

        public async UniTask ShowAsync(CancellationToken cancellationToken = default)
        {
            using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(_tokenSource.Token, cancellationToken);

            await BeforeShowAsync(tokenSource.Token);
            await View.ShowAsync(tokenSource.Token);
            await AfterShowAsync(tokenSource.Token);
        }

        public async UniTask HideAsync(CancellationToken cancellationToken = default)
        {
            using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(_tokenSource.Token, cancellationToken);

            await BeforeHideAsync(tokenSource.Token);
            await View.HideAsync(tokenSource.Token);
            await AfterHideAsync(tokenSource.Token);
        }

        public void Dispose()
        {
            _tokenSource.Cancel();
            _tokenSource.Dispose();

            OnDispose();
        }

        protected virtual void OnBindModel(TWindowModel windowModel)
        {
        }

        protected virtual void OnBindView(TWindowView windowView)
        {
        }

        protected virtual UniTask BeforeShowAsync(CancellationToken cancellationToken)
            => UniTask.CompletedTask;

        protected virtual UniTask AfterShowAsync(CancellationToken cancellationToken)
            => UniTask.CompletedTask;

        protected virtual UniTask BeforeHideAsync(CancellationToken cancellationToken)
            => UniTask.CompletedTask;

        protected virtual UniTask AfterHideAsync(CancellationToken cancellationToken)
            => UniTask.CompletedTask;

        protected virtual void OnDispose()
        {
        }
    }
}