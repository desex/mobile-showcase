using System;
using R3;

namespace Sotbify.MobileApp.UI.Window
{
    public static class WindowServiceR3Extensions
    {
        public static Observable<Type> OpenedAsObservable(this IWindowService windowService)
        {
            return windowService is not null
                ? Observable.FromEvent<Type>(h => windowService.Opened += h, h => windowService.Opened -= h)
                : throw new ArgumentNullException(nameof(windowService));
        }
        
        public static Observable<Type> ClosedAsObservable(this IWindowService windowService)
        {
            return windowService is not null
                ? Observable.FromEvent<Type>(h => windowService.Closed += h, h => windowService.Closed -= h)
                : throw new ArgumentNullException(nameof(windowService));
        }

        public static Observable<bool> IsOpenedAsObservable<TWindowModel>(this IWindowService windowService)
            where TWindowModel : class
        {
            if(windowService is null)
                throw new ArgumentNullException(nameof(windowService));
            
            var typeWindowModel = typeof(TWindowModel);

            var openedObservable = windowService.OpenedAsObservable()
                .Where(typeWindowModel, static (openedWindowModelType, windowModelType) => openedWindowModelType == windowModelType)
                .Select(static _ => true);
            
            var closedObservable = windowService.ClosedAsObservable()
                .Where(typeWindowModel, static (openedWindowModelType, windowModelType) => openedWindowModelType == windowModelType)
                .Select(static _ => false);

            return openedObservable.Merge(closedObservable);
        }

        public static ReadOnlyReactiveProperty<bool> IsOpenedAsReadOnlyReactiveProperty<TWindowModel>(this IWindowService windowService)
            where TWindowModel : class
        {
            if (windowService is null)
                throw new ArgumentNullException(nameof(windowService));

            return windowService.IsOpenedAsObservable<TWindowModel>()
                .ToReadOnlyReactiveProperty(windowService.IsOpen<TWindowModel>());
        }
    }
}