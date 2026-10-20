using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sotbify.MobileApp.UI.Layout
{
    public sealed class LerpLayoutElement : UIBehaviour, ILayoutElement
    {
        private const string WidthGroup = "Width";
        private const string HeightGroup = "Height";

        [SerializeField] private RectTransform _fromRectTransform;
        [SerializeField] private RectTransform _toRectTransform;
        [Space] 
        [Range(0f, 1f)] 
        [SerializeField] private float _time;

        [BoxGroup(WidthGroup)] 
        [SerializeField] private bool _useMinWidth = true;
        [BoxGroup(WidthGroup)] 
        [SerializeField] private bool _usePreferredWidth = true;
        [BoxGroup(WidthGroup)]
        [SerializeField] private bool _useFlexibleWidth = true;

        [BoxGroup(HeightGroup)] 
        [SerializeField] private bool _useMinHeight = true;
        [BoxGroup(HeightGroup)]
        [SerializeField] private bool _usePreferredHeight = true;
        [BoxGroup(HeightGroup)]
        [SerializeField] private bool _useFlexibleHeight = true;

        [SerializeField] private int _layoutPriority = 0;

        public RectTransform From
        {
            get => _fromRectTransform;
            set => SetClass(ref _fromRectTransform, value);
        }

        public RectTransform To
        {
            get => _toRectTransform;
            set => SetClass(ref _toRectTransform, value);
        }

        public float Time
        {
            get => _time;
            set => SetStruct(ref _time, value);
        }

        public bool UseMinWidth
        {
            get => _useMinWidth;
            set => SetStruct(ref _useMinWidth, value);
        }

        public bool UsePreferredWidth
        {
            get => _usePreferredWidth;
            set => SetStruct(ref _usePreferredWidth, value);
        }

        public bool UseFlexibleWidth
        {
            get => _useFlexibleWidth;
            set => SetStruct(ref _useFlexibleWidth, value);
        }

        public bool UseMinHeight
        {
            get => _useMinHeight;
            set => SetStruct(ref _useMinHeight, value);
        }

        public bool UsePreferredHeight
        {
            get => _usePreferredHeight;
            set => SetStruct(ref _usePreferredHeight, value);
        }

        public bool UseFlexibleHeight
        {
            get => _useFlexibleHeight;
            set => SetStruct(ref _useFlexibleHeight, value);
        }

        public float minWidth
        {
            get
            {
                if (_useMinWidth == false)
                    return -1;
                
                var fromMinWidth = GetMinWidth(_fromRectTransform);
                var toMinWidth = GetMinWidth(_toRectTransform);
                return Mathf.Lerp(fromMinWidth, toMinWidth, _time);
            }
        }

        public float preferredWidth
        {
            get
            {
                if (_usePreferredWidth == false)
                    return -1;
                
                var fromMinWidth = GetPreferredWidth(_fromRectTransform);
                var toMinWidth = GetPreferredWidth(_toRectTransform);
                return Mathf.Lerp(fromMinWidth, toMinWidth, _time);
            }
        }

        public float flexibleWidth
        {
            get
            {
                if (_useFlexibleWidth == false)
                    return -1;
                
                var fromMinWidth = GetFlexibleWidth(_fromRectTransform);
                var toMinWidth = GetFlexibleWidth(_toRectTransform);
                return Mathf.Lerp(fromMinWidth, toMinWidth, _time);
            }
        }

        public float minHeight
        {
            get
            {
                if (_useMinHeight == false)
                    return -1;
                
                var fromMinWidth = GetMinHeight(_fromRectTransform);
                var toMinWidth = GetMinHeight(_toRectTransform);
                return Mathf.Lerp(fromMinWidth, toMinWidth, _time);
            }
        }

        public float preferredHeight
        {
            get
            {
                if (_usePreferredHeight == false)
                    return -1;
                
                var fromMinWidth = GetPreferredHeight(_fromRectTransform);
                var toMinWidth = GetPreferredHeight(_toRectTransform);
                return Mathf.Lerp(fromMinWidth, toMinWidth, _time);
            }
        }

        public float flexibleHeight
        {
            get
            {
                if (_useFlexibleHeight == false)
                    return -1;
                
                var fromMinWidth = GetFlexibleHeight(_fromRectTransform);
                var toMinWidth = GetFlexibleHeight(_toRectTransform);
                return Mathf.Lerp(fromMinWidth, toMinWidth, _time);
            }
        }

        public int layoutPriority => _layoutPriority;

        public void CalculateLayoutInputHorizontal()
        {
        }

        public void CalculateLayoutInputVertical()
        {
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if(UnityEngine.Application.isPlaying)
                return;

            SetDirty();
        }
#endif

        protected override void OnDidApplyAnimationProperties()
        {
            SetDirty();
        }

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
        private float GetMinWidth(RectTransform rectTransform)
        {
            return rectTransform.GetMinWidth
            (
                this,
                static (keeper, element) => ReferenceEquals(keeper, element) == false,
                0,
                out _
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetPreferredWidth(RectTransform rectTransform)
        {
            return rectTransform.GetPreferredWidth
            (
                this,
                static (keeper, element) => ReferenceEquals(keeper, element) == false,
                0,
                out _
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetFlexibleWidth(RectTransform rectTransform)
        {
            return rectTransform.GetFlexibleWidth
            (
                this,
                static (keeper, element) => ReferenceEquals(keeper, element) == false,
                0,
                out _
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetMinHeight(RectTransform rectTransform)
        {
            return rectTransform.GetMinHeight
            (
                this,
                static (keeper, element) => ReferenceEquals(keeper, element) == false,
                0,
                out _
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetPreferredHeight(RectTransform rectTransform)
        {
            return rectTransform.GetPreferredHeight
            (
                this,
                static (keeper, element) => ReferenceEquals(keeper, element) == false,
                0,
                out _
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetFlexibleHeight(RectTransform rectTransform)
        {
            return rectTransform.GetFlexibleHeight
            (
                this,
                static (keeper, element) => ReferenceEquals(keeper, element) == false,
                0,
                out _
            );
        }
    }
}