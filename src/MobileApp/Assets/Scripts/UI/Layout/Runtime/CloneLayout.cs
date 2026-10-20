using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sotbify.MobileApp.UI.Layout
{
    public sealed class CloneLayout : UIBehaviour, ILayoutElement
    {
        private const string WidthGroup = "Width";
        private const string HeightGroup = "Height";
        
        [SerializeField] private RectTransform _target;
        
        [BoxGroup(WidthGroup)] 
        [SerializeField] private bool _cloneMinWidth = true;
        [BoxGroup(WidthGroup)] 
        [SerializeField] private bool _clonePreferredWidth = true;
        [BoxGroup(WidthGroup)]
        [SerializeField] private bool _cloneFlexibleWidth = true;

        [BoxGroup(HeightGroup)] 
        [SerializeField] private bool _cloneMinHeight = true;
        [BoxGroup(HeightGroup)]
        [SerializeField] private bool _clonePreferredHeight = true;
        [BoxGroup(HeightGroup)]
        [SerializeField] private bool _cloneFlexibleHeight = true;
        
        [SerializeField] private int _layoutPriority = 0;

        private RectTransform _selfRectTransform;
        
        public RectTransform Target
        {
            get => _target;
            set => SetClass(ref _target, value);
        }

        public float minWidth => _cloneMinWidth && RectTransformIsSelf(_target) == false ? LayoutUtility.GetMinWidth(_target) : -1f;
        public float preferredWidth => _clonePreferredWidth && RectTransformIsSelf(_target) == false ? LayoutUtility.GetPreferredWidth(_target) : -1f;
        public float flexibleWidth => _cloneFlexibleWidth && RectTransformIsSelf(_target) == false ? LayoutUtility.GetFlexibleWidth(_target) : -1f;
        
        public float minHeight => _cloneMinHeight && RectTransformIsSelf(_target) == false ? LayoutUtility.GetMinHeight(_target) : -1f;
        public float preferredHeight => _clonePreferredHeight && RectTransformIsSelf(_target) == false ? LayoutUtility.GetPreferredHeight(_target) : -1f;
        public float flexibleHeight => _cloneFlexibleHeight && RectTransformIsSelf(_target) == false ? LayoutUtility.GetFlexibleHeight(_target) : -1f;

        public int layoutPriority => _layoutPriority;
        
        public void CalculateLayoutInputHorizontal() { }

        public void CalculateLayoutInputVertical() { }
        
        private void SetStruct<T>(ref T currentValue, T newValue) where T : struct
        {
            if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
                return;

            currentValue = newValue;
            SetDirty();
        }

        private void SetClass<T>(ref T currentValue, T newValue) where T : class
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool RectTransformIsSelf(RectTransform rectTransform) => 
            GetSelfRectTransform() == rectTransform;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private RectTransform GetSelfRectTransform()
        {
            if (_selfRectTransform == null)
                _selfRectTransform = transform as RectTransform;
            
            return _selfRectTransform;
        }
    }
}