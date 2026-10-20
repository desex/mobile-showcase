#nullable enable

using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using LeTai.Asset.TranslucentImage;
using R3;
using Sotbify.MobileApp.Relic;

namespace Sotbify.MobileApp.HomeWindow
{
    public interface IHome
    {
        TranslucentImageSource TranslucentImageSource { get; }
        ReadOnlyReactiveProperty<IReadOnlyList<IRelicInstance?>> Items { get; }
        Observable<Unit> EditActiveSetObservable { get; }
        Observable<Unit> CasinoOpenedObservable { get; }

        UniTask OpenCasino(CancellationToken cancellationToken);
        UniTask LoadItemsAsync(CancellationToken cancellationToken = default);
        
        void EditActiveSet();
    }
}