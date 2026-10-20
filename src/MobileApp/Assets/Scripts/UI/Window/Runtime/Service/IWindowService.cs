using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.UI.Window.Presenter;
using Sotbify.MobileApp.UI.Window.View;
using UnityEngine;

namespace Sotbify.MobileApp.UI.Window
{
    public interface IWindowService
    {
        event Action<Type> Opened;
        event Action<Type> Closed;
        
        void Bind<TWindowModel, TWindowPresenter, TWindowView>()
            where TWindowModel : class
            where TWindowPresenter : IWindowPresenter<TWindowModel, TWindowView>
            where TWindowView : IWindowView;

        void Unbind<TWindowModel>() where TWindowModel : class;

        bool IsOpen<TWindowModel>() where TWindowModel : class;
        
        UniTask<TWindowModel> OpenAsync<TWindowModel>(CancellationToken cancellationToken = default) where TWindowModel : class;

        UniTask<TWindowModel> OpenAsync<TWindowModel>
        (
            Transform parent,
            bool instantiateInWorldSpace = false,
            CancellationToken cancellationToken = default
        ) where TWindowModel : class;
        
        GameObject GetView<TWindowModel>() where TWindowModel : class;

        UniTask CloseAsync<TWindowModel>(CancellationToken cancellationToken = default) where TWindowModel : class;
    }
}