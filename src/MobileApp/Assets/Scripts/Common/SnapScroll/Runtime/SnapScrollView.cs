using System;
using LitMotion;
using R3;
using Sotbify.MobileApp.UI.Common;
using Sotbify.MobileApp.Utils.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sotbifay.MobileApp.Common.SnapScroll
{
    public abstract class SnapScrollView : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        private readonly ReactiveProperty<int> _snappedItemIndex = new(0);

        [SerializeField] private ScrollRect _scrollRect;

        private MotionHandle _focusMotionHandle;
        private bool _currentlySnapping;
        
        public RectTransform Content => _scrollRect != null ? _scrollRect.content : null;

        protected abstract Ease SnapEase { get; set; }
        protected abstract float SnapDuration { get; set; }

        public ReadOnlyReactiveProperty<int> SnappedItemIndex => _snappedItemIndex;

        private Vector2 _startNormalizedPosition;

        public void SnapToIndex(int index)
        {
            _snappedItemIndex.Value = Mathf.Clamp(index, 0, _scrollRect.content.childCount - 1);

            if (_focusMotionHandle.IsActive())
                _focusMotionHandle.Cancel();

            var contentRectTransform = _scrollRect.content
                .GetChild(_snappedItemIndex.Value)
                .AsRectTransform();

            var currentNormalizedPosition = _scrollRect.normalizedPosition;
            var targetNormalizedPosition = _scrollRect.CalculateFocusedScrollPosition
            (
                contentRectTransform.localPosition,
                new Vector2(0f, 0.5f)
            );

            _focusMotionHandle = LMotion
                .Create(currentNormalizedPosition, targetNormalizedPosition, SnapDuration)
                .WithEase(SnapEase)
                .Bind(_scrollRect, (normalizedPosition, scrollRect) => scrollRect.normalizedPosition = normalizedPosition)
                .AddTo(_scrollRect);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_focusMotionHandle.IsActive())
                _focusMotionHandle.Cancel();

            _startNormalizedPosition = _scrollRect.normalizedPosition;
        }

        private void OnDestroy()
        {
            _snappedItemIndex.Dispose(true);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_currentlySnapping == false)
            {
                var snappedItemIndex = CalculateNextItemIndex();
                _snappedItemIndex.Value = Mathf.Clamp(snappedItemIndex, 0, _scrollRect.content.childCount - 1);
            }

            SnapToIndex(_snappedItemIndex.CurrentValue);
        }

        private int CalculateNextItemIndex()
        {
            var currentNormalizedPosition = _scrollRect.normalizedPosition;
            var delta = currentNormalizedPosition - _startNormalizedPosition;
            var content = _scrollRect.content;
            var contentTransform = content.transform;

            var item = contentTransform.GetChild(_snappedItemIndex.Value) as RectTransform;
            var threshold = 1f / contentTransform.childCount * 0.1f;
            var snappedItemIndex = _snappedItemIndex.Value;

            if (item == null)
                return -1;

            if (delta.x < -threshold)
            {
                snappedItemIndex--;
            }
            else if (delta.x > threshold)
            {
                snappedItemIndex++;
            }

            return snappedItemIndex;
        }
    }
}