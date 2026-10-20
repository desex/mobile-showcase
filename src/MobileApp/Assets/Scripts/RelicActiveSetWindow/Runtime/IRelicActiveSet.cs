using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;

namespace Sotbify.MobileApp.RelicActiveSetWindow
{
    public interface IRelicActiveSet
    {
        public Observable<Unit> CloseRelicChoseObservable { get; }
        public Observable<int> EquipRelicObservable { get; }
        public Observable<Unit> UpdateActiveSet { get; }
        public ReadOnlyReactiveProperty<bool> IsEnoughToRandomize { get; }
        public IReadOnlyList<RelicLineModel> RelicLines { get; }
        public UniTask OpenRelicChooseAsync(int id, CancellationToken cancellationToken = default);
        public UniTask LoadActiveSetAsync(CancellationToken cancellationToken = default);
        public UniTask CloseRelicChooseAsync(CancellationToken cancellationToken = default);
        public UniTask EquipRelicAsync(int relicId, int selectedPosition, CancellationToken cancellationToken);
        public UniTask RandomizeActiveSet(CancellationToken cancellationToken = default);
    }
}