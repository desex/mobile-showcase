using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;

namespace Sotbify.MobileApp.RelicListWindow
{
    public interface IRelicListWindow
    {
        ReadOnlyReactiveProperty<IReadOnlyList<RelicModel>> Relics { get; }
        ReadOnlyReactiveProperty<int> TotalRelicsCount { get; }
        Observable<int> OpenDescriptionObservable { get; }
        
        UniTask LoadRelics(CancellationToken cancellationToken = default);
        void OpenDescription(int modelId);
    }
}