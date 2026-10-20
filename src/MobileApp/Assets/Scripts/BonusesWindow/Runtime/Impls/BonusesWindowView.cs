using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.BonusesWindow.Core;
using Sotbify.MobileApp.HomeWindow.Filter;
using Sotbify.MobileApp.UI.Common.Components.Filter;
using Sotbify.MobileApp.UI.Window.View;
using Sotbify.MobileApp.Utils.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.BonusesWindow
{
    public class BonusesWindowView : FadeWindowView
    {
        [SerializeField] private BonusesScroll _bonusesScroll;
        [SerializeField] private FilterListView<EBonusType> _filterList;
        [SerializeField] private GameObject _noBonusesView;
        [SerializeField] private Button _openChatButton;

        private CancellationTokenSource _cancellationTokenSource;

        private FilterItem<EBonusType> _activeFilterItem;

        public BonusesScroll BonusesScroll => _bonusesScroll;
        public FilterListView<EBonusType> Filters => _filterList;

        public Observable<Unit> OpenChatObservable => _openChatButton.OnClickAsObservable();

        public bool IsNoBonusesActive
        {
            get => _noBonusesView.activeSelf;
            set => _noBonusesView.SetActive(value);
        }

        protected override UniTask ShowInternalAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void ActivateFilter(FilterItem<EBonusType> filterItem)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}