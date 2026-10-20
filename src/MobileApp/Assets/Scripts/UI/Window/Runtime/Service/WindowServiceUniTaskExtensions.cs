using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sotbify.MobileApp.UI.Window
{
    public static class WindowServiceUniTaskExtensions
    {
        public static UniTask SetActiveAsync<TWindowModel>
        (
            this IWindowService windowService,
            bool isActive,
            CancellationToken cancellationToken = default
        ) where TWindowModel : class
        {
            if (windowService is null)
                throw new ArgumentNullException(nameof(windowService));

            if (windowService.IsOpen<TWindowModel>() == isActive)
                return UniTask.CompletedTask;

            return isActive
                ? windowService.OpenAsync<TWindowModel>(cancellationToken)
                : windowService.CloseAsync<TWindowModel>(cancellationToken);
        }

        public static UniTask SetActiveAsync<TWindowModel>
        (
            this IWindowService windowService,
            bool isActive,
            Transform parent,
            bool instantiateInWorldSpace = false,
            CancellationToken cancellationToken = default
        ) where TWindowModel : class
        {
            if (windowService is null)
                throw new ArgumentNullException(nameof(windowService));

            if (windowService.IsOpen<TWindowModel>() == isActive)
                return UniTask.CompletedTask;

            return isActive
                ? windowService.OpenAsync<TWindowModel>(parent, instantiateInWorldSpace, cancellationToken)
                : windowService.CloseAsync<TWindowModel>(cancellationToken);
        }
    }
}