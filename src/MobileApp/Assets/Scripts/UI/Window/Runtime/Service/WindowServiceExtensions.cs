using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sotbify.MobileApp.UI.Window
{
    public static class WindowServiceExtensions
    {
        public static UniTask SafeOpenAsync<TWindowModel>(this IWindowService windowService, CancellationToken cancellationToken = default)
            where TWindowModel : class
        {
            if (windowService is null)
                throw new ArgumentNullException(nameof(windowService));

            return windowService.IsOpen<TWindowModel>() == false
                ? windowService.OpenAsync<TWindowModel>(cancellationToken)
                : UniTask.CompletedTask;
        }

        public static UniTask SafeOpenAsync<TWindowModel>
        (
            this IWindowService windowService,
            Transform parent,
            bool instantiateInWorldSpace = false,
            CancellationToken cancellationToken = default
        ) where TWindowModel : class
        {
            if (windowService is null)
                throw new ArgumentNullException(nameof(windowService));

            return windowService.IsOpen<TWindowModel>() == false
                ? windowService.OpenAsync<TWindowModel>(parent, instantiateInWorldSpace, cancellationToken)
                : UniTask.CompletedTask;
        }

        public static UniTask SafeCloseAsync<TWindowModel>(this IWindowService windowService, CancellationToken cancellationToken = default)
            where TWindowModel : class
        {
            if (windowService is null)
                throw new ArgumentNullException(nameof(windowService));

            return windowService.IsOpen<TWindowModel>()
                ? windowService.CloseAsync<TWindowModel>(cancellationToken)
                : UniTask.CompletedTask;
        }
    }
}