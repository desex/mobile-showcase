using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sotbify.MobileApp.UI.Layout
{
    [ExecuteAlways]
    public sealed class CloneRectTransformAsLayout : UIBehaviour, ILayoutElement
    {
        private const string WidthGroup = "Width";
        private const string HeightGroup = "Height";

        [SerializeField] private RectTransform _rectTransform;

        [BoxGroup(WidthGroup)] 
        [SerializeField] private bool _cloneAsMinWidth = true;
        [BoxGroup(WidthGroup)] 
        [SerializeField] private bool _cloneAsPreferredWidth = true;

        [BoxGroup(HeightGroup)] 
        [SerializeField] private bool _cloneAsMinHeight = true;
        [BoxGroup(HeightGroup)]
        [SerializeField] private bool _cloneAsPreferredHeight = true;

        [SerializeField] private int _layoutPriority = 2;

#if UNITY_EDITOR
        [ShowInInspector, ReadOnly] private Vector2 DebugRectTransformSize => TryGetRectTransformSize(out var size) ? size : Vector2.zero;
#endif

        private Vector2 _lastSize;

        public RectTransform RectTransform
        {
            get => _rectTransform;
            set => SetClass(ref _rectTransform, value);
        }

        public bool CloneAsMinWidth
        {
            get => _cloneAsMinWidth;
            set => SetStruct(ref _cloneAsMinWidth, value);
        }
        
        public bool CloneAsPreferredWidth
        {
            get => _cloneAsPreferredWidth;
            set => SetStruct(ref _cloneAsPreferredWidth, value);
        }
        
        public bool CloneAsMinHeight
        {
            get => _cloneAsMinHeight;
            set => SetStruct(ref _cloneAsMinHeight, value);
        }
        
        public bool CloneAsPreferredHeight
        {
            get => _cloneAsPreferredHeight;
            set => SetStruct(ref _cloneAsPreferredHeight, value);
        }
        
        public float minWidth => _cloneAsMinWidth && TryGetRectTransformSize(out var size)
            ? size.x
            : -1;

        public float preferredWidth => _cloneAsPreferredWidth && TryGetRectTransformSize(out var size)
            ? size.x
            : -1;

        public float flexibleWidth => -1;

        public float minHeight => _cloneAsMinHeight && TryGetRectTransformSize(out var size)
            ? size.y
            : -1;

        public float preferredHeight => _cloneAsPreferredHeight && TryGetRectTransformSize(out var size)
            ? size.y
            : -1;

        public float flexibleHeight => -1;

        public int layoutPriority => _layoutPriority;

        public void CalculateLayoutInputHorizontal() { }

        public void CalculateLayoutInputVertical() { }

        private void Update()
        {
            if (TryGetRectTransformSize(out var currentSize) == false)
                return;

            if (currentSize == _lastSize)
                return;

            _lastSize = currentSize;
            SetDirty();
        }

        private bool TryGetRectTransformSize(out Vector2 size)
        {
            size = Vector2.zero;

            if (_rectTransform == null)
                return false;

            var rect = _rectTransform.rect;
            size = rect.size;
            return true;
        }
        
        private void SetStruct<T>(ref T currentValue, T newValue) where T : struct
        {
            if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
                return;

            currentValue = newValue;
            SetDirty();
        }
        
        public void SetClass<T>(ref T currentValue, T newValue) where T : class
        {
            if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
                return;

            currentValue = newValue;
            SetDirty();
        }
        
        private void SetDirty()
        {
            if (IsActive() == false)
                return;
            
            LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
        }
    }
}