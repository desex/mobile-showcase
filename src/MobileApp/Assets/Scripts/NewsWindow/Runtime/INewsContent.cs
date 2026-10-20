using System.Threading;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using Sotbify.MobileApp.NewsWindow.Models;
using UnityEngine;

namespace Sotbify.MobileApp.NewsWindow
{
    public interface INewsContent
    {
        IReadOnlyObservableList<INewsContentItemModel> Items { get; }
        public Observable<string> Title { get; }
        public Observable<Sprite> Image { get; }
        public Observable<Unit> StartLoadingTitleImage { get; }
        public Observable<Unit> StopLoadingTitleImage { get; }
        public Observable<Unit> CloseObservable { get; }
        Observable<IReadOnlyCollection<INewsContentItemModel>> ItemsLoaded { get; }
        ReadOnlyReactiveProperty<bool> CanLoadNext { get; }
        ReadOnlyReactiveProperty<bool> CanLoadPrevious { get; }
        ReadOnlyReactiveProperty<bool> IsLoading { get; }

        UniTask LoadContent(int id, CancellationToken cancellationToken = default);
        UniTask LoadNext(CancellationToken cancellationToken = default);
        UniTask LoadPrevious(CancellationToken cancellationToken = default);
        void MarkViewedToEnd();
        public void CloseContent();
        void Clear();
    }
}
