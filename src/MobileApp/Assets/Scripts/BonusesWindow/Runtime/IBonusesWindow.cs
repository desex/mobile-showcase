using System.Threading;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using Sotbify.MobileApp.BonusesWindow.Core.Models;
using Sotbify.MobileApp.UI.Common.Components.Filter;
using Sotbify.MobileApp.Utils.Enums;

namespace Sotbify.MobileApp.BonusesWindow
{
    public interface IBonusesWindow
    {
        public IReadOnlyObservableList<IBonusModel> Promocodes { get; }
        public IReadOnlyObservableList<FilterData<EBonusType>> Filters { get; }
        public ReadOnlyReactiveProperty<bool> IsNoBonusActive { get; }

        public UniTask LoadBonusesAsync(CancellationToken cancellationToken = default);
        public UniTask LoadFiltersAsync(CancellationToken cancellationToken = default);
        
        public void Clear();
        public void Copy(string promocode);
        public void FilterItems(EBonusType chosenType);
        public UniTask OpenUrlAsync(string url, CancellationToken cancellationToken = default);

        public void OpenChat();
    }
}