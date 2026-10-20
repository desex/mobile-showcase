using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sotbify.MobileApp.UI.Layout
{
    public sealed class MaxPreferredLayout : UIBehaviour, ILayoutElement
    {
        private const string WidthGroup = "Width";
        private const string HeightGroup = "Height";

        [BoxGroup(WidthGroup)] 
        [SerializeField] private bool _useMaxWidth = false;
        [BoxGroup(WidthGroup), ShowIf(nameof(_useMaxWidth))] 
        [SerializeField] private float _maxWidth = 0f;

        [BoxGroup(HeightGroup)]
        [SerializeField] private bool _useMaxHeight = false;
        [BoxGroup(HeightGroup), ShowIf(nameof(_useMaxHeight))]
        [SerializeField] private float _maxHeight = 0f;

        [SerializeField] private int _layoutPriority = 2;

        private RectTransform _rectTransform;

        public float minWidth => GetRectTransform().GetLayoutProperty
        (
            this,
            static (@this, element) => @this != element as MaxPreferredLayout,
            static (_, element) => element.minWidth,
            0f,
            out _
        );

        public float preferredWidth => GetRectTransform().GetLayoutProperty
        (
            this,
            static (@this, element) => @this != element as MaxPreferredLayout,
            static (@this, element) => @this._useMaxWidth ? Mathf.Min(element.preferredWidth, @this._maxWidth) : element.preferredWidth,
            0f,
            out _
        );

        public float flexibleWidth => GetRectTransform().GetLayoutProperty
        (
            this,
            static (@this, element) => @this != element as MaxPreferredLayout,
            static (_, element) => element.flexibleWidth,
            0f,
            out _
        );

        public float minHeight => GetRectTransform().GetLayoutProperty
        (
            this,
            static (@this, element) => @this != element as MaxPreferredLayout,
            static (_, element) => element.minHeight,
            0f,
            out _
        );

        public float preferredHeight => GetRectTransform().GetLayoutProperty
        (
            this,
            static (@this, element) => @this != element as MaxPreferredLayout,
            static (@this, element) => @this._useMaxHeight ? Mathf.Min(element.preferredHeight, @this._maxHeight) : element.preferredHeight,
            0f,
            out _
        );

        public float flexibleHeight => GetRectTransform().GetLayoutProperty
        (
            this,
            static (@this, element) => @this != element as MaxPreferredLayout,
            static (_, element) => element.flexibleHeight,
            0f,
            out _
        );

        public int layoutPriority => _layoutPriority;

        public void CalculateLayoutInputHorizontal()
        {
        }

        public void CalculateLayoutInputVertical()
        {
        }

        private RectTransform GetRectTransform() =>
            _rectTransform != null ? _rectTransform : _rectTransform = transform as RectTransform;
    }
}