using System;
using System.Collections.Generic;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sotbify.MobileApp.UI.Common.Components.DropDown
{
    public class DropDown : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private string _placeholder;
        [SerializeField] private bool _canUnselect = true;
        [SerializeField] private TMP_Text _caption;
        [SerializeField] private GameObject _menu;
        [SerializeField] private DropDownItemsCollectionView _itemsCollectionView;
        [SerializeField] private bool _useOverlayCanvas;
        [SerializeField] private DropDownItemView _defaultItem;
        [SerializeField] private List<DropDownItemView> _defaultItems;

        [SerializeField, ShowIf(nameof(_useOverlayCanvas))]
        private RectTransform _overlayCanvas;

        private readonly List<DropDownItemView> _items = new();
        private CompositeDisposable _disposable;

        public readonly ReactiveProperty<bool> IsEnabled = new(false);
        public readonly ReactiveProperty<int> SelectedOption = new(-1);
        public readonly ReactiveProperty<string> SelectedOptionName = new();

        private void Awake()
        {
            foreach (var defaultItem in _defaultItems)
                AddDefault(defaultItem);
            
            SelectDefault(_defaultItem);
        }

        private void OnEnable()
        {
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();

            IsEnabled.Subscribe(this, (state, dropDown) => dropDown.SetActiveMenu(state))
                .AddTo(_disposable);

            SelectedOption.Subscribe(this, (optionId, dropDown) => dropDown.OnSelecedOptionChanged(optionId))
                .AddTo(_disposable); 
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
            _disposable = null;
        }

        private void OnSelecedOptionChanged(int optionId)
        {
            var itemViews = _itemsCollectionView.Views;
            for (var i = 0; i < itemViews.Count; i++)
            {
                itemViews[i].IsSelected.Value = i == optionId;
            }

            var targetText = optionId == -1 ? _placeholder : itemViews[optionId].Caption.Value;
            _caption.SetText(targetText);
        }

        public void AddOptions(params string[] options)
        {
            for (var i = 0; i < options.Length; i++)
            {
                AddOption(options[i]);
            }
        }

        private void SelectDefault(DropDownItemView item)
        {
            if (item.IsSelected.CurrentValue == false)
                OnOptionClicked(item);
        }

        [Button]
        public void AddOption(string caption)
        {
            var itemView = _itemsCollectionView.CreateView();
            itemView.Id = _itemsCollectionView.Views.Count - 1;
            itemView.Caption.Value = caption;
            itemView.ClickObservable.Subscribe(this, (view, dropDown) => dropDown.OnOptionClicked(view));
            _items.Add(itemView);
        }

        private void AddDefault(DropDownItemView item)
        {
            item.InitializeAsDefault();
            _itemsCollectionView.AddView(item);
            item.Id = _itemsCollectionView.Views.Count - 1;
            item.ClickObservable.Subscribe(this, (view, dropDown) => dropDown.OnOptionClicked(view));
            _items.Add(item);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            IsEnabled.Value = !IsEnabled.Value;
        }

        public void SetActiveOption(int index, bool state)
        {
            var itemView = _items[index];
            itemView.SetActive(state);
        }

        private void OnOptionClicked(DropDownItemView view)
        {
            var viewId = view.Id;
            var viewName = view.Caption.Value;
            
            if (_canUnselect)
            {
                SelectedOption.Value = SelectedOption.Value == viewId ? -1 : viewId;
                SelectedOptionName.Value = SelectedOptionName.Value == viewName ? string.Empty : viewName;
            }
            else
            {
                SelectedOption.Value = viewId;
                SelectedOptionName.Value = viewName;
            }
        }

        private void SetActiveMenu(bool state)
        {
            if (_useOverlayCanvas)
            {
                _menu.transform.SetParent(state ? _overlayCanvas : transform);
            }

            _menu.SetActive(state);
        }

        private void OnValidate()
        {
            if (_caption == null)
            {
                return;
            }

            _caption.SetText(_placeholder);
        }
    }
}