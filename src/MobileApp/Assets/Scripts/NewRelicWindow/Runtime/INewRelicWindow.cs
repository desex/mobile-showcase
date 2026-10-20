using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace Sotbify.MobileApp.NewRelicWindow
{
    public interface INewRelicWindow
    {
        ReadOnlyReactiveProperty<Texture> ItemTexture { get; }
        ReadOnlyReactiveProperty<string> ItemName { get; }
        ReadOnlyReactiveProperty<string> ItemDescription { get; }
        ReadOnlyReactiveProperty<int> ItemRarity { get; }
        Observable<Unit> CloseObservable { get; }

        UniTask<bool> CheckDailyBonusAsync(CancellationToken cancellationToken = default);
        UniTask LoadInfoAsync(CancellationToken cancellationToken = default);
        void Close();
        UniTask ReleaseInfoAsync(CancellationToken cancellationToken = default);
    }
}