using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using Sotbify.MobileApp.Relic;
using Sotbify.MobileApp.RelicChooseWindow.Adapter;
using Sotbify.MobileApp.RelicsDescriptionWindow;

namespace Sotbify.MobileApp.RelicChooseWindow
{
    public interface IRelicChooseModel
    {
        ReadOnlyReactiveProperty<IReadOnlyCollection<int>> EquippedRelics { get; }
        ReadOnlyReactiveProperty<RelicDescriptionModel> RelicDescription { get; }
        ReadOnlyReactiveProperty<IRelicInstance> RelicInstance { get; }
        Observable<int> EquipRelicObservable { get; }
        Observable<Unit> CloseRelicChoseObservable { get; }
        IReadOnlyObservableList<RelicModel> Relics { get; }

        void SetEquippedRelics(IEnumerable<int> equippedRelicIds);
        void Equip();
        UniTask OpenDescriptionAsync(int id, IRelicInstance relicInstance, CancellationToken cancellationToken = default);
        void Close();
        UniTask LoadRelics(CancellationToken cancellationToken = default);
        void Reset();
    }
}