using System;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sotbify.MobileApp.UI.Common.Components.DropDown
{
    public sealed class DropDownItemView : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerClickHandler,
        IPointerDownHandler
    {
        [SerializeField] private TMP_Text _caption;
        [SerializeField] private Graphic _colorGraphics;
        [SerializeField] private Color _normalColor;
        [SerializeField] private Color _selectedColor;
        [SerializeField] private Color _hoveredColor;
        [SerializeField] private Color _pressedColor;

        public readonly ReactiveProperty<bool> IsSelected = new (false);
        private readonly Subject<DropDownItemView> _clickSubject = new();

        public Observable<DropDownItemView> ClickObservable => _clickSubject;
        public readonly ReactiveProperty<string> Caption = new ();
        private CompositeDisposable _disposable;
        public int Id { get; set; }

        public void InitializeAsDefault()
        {
            Caption.Value = _caption.text;
        }

        private void OnEnable()
        {
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();

            IsSelected.Subscribe(this, (_, view) => view.SetColor(GetNormalColor()))
                .AddTo(_disposable);

            Caption.Subscribe(this, (captionText, view) => view._caption.SetText(captionText))
                .AddTo(_disposable);
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
            _disposable = null;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SetColor(_hoveredColor);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetColor(GetNormalColor());
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _clickSubject.OnNext(this);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            SetColor(_pressedColor);
        }

        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
        }

        private void SetColor(Color color)
        {
            _colorGraphics.color = color;
        }

        private Color GetNormalColor()
        {
            return IsSelected.Value ? _selectedColor : _normalColor;
        }
    }
}