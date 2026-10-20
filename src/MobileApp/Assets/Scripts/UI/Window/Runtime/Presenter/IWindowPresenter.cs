using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.UI.Window.View;

namespace Sotbify.MobileApp.UI.Window.Presenter
{
    public interface IWindowPresenter : IDisposable
    {
        void Initialize();
        
        void BindModel(object windowModel);
        void BindView(IWindowView windowView);

        UniTask ShowAsync(CancellationToken cancellationToken = default);
        UniTask HideAsync(CancellationToken cancellationToken = default);
    }

    public interface IWindowPresenter<in TWindowModel, in TWindowView> : IWindowPresenter
        where TWindowModel : class
        where TWindowView : IWindowView
    {
        void BindModel(TWindowModel windowModel);
        void BindView(TWindowView windowView);
    }
}