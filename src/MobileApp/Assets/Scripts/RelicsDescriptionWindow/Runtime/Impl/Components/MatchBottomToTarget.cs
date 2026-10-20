using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.RelicsDescriptionWindow
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public sealed class MatchBottomToTarget : MonoBehaviour, ILayoutSelfController
    {
        [SerializeField] RectTransform _target;
        [SerializeField] float _extraBottom;
        
        private RectTransform _rectTransform;
        private DrivenRectTransformTracker _tracker;
        
        public RectTransform RectTransform => _rectTransform ? _rectTransform : _rectTransform = (RectTransform)transform;

        private void OnEnable() => 
            SetDirty();

        private void OnDisable()
        {
            _tracker.Clear();
            LayoutRebuilder.MarkLayoutForRebuild(RectTransform);
        }

#if UNITY_EDITOR
        private void OnValidate() => SetDirty();
#endif

        private void SetDirty()
        {
            if (isActiveAndEnabled)
                LayoutRebuilder.MarkLayoutForRebuild(RectTransform);
        }

        public void SetLayoutHorizontal() { }

        public void SetLayoutVertical()
        {
            _tracker.Clear();
            if (_target == null) 
                return;

            var parent = RectTransform.parent as RectTransform;
            if (parent == null) 
                return;

            _tracker.Add(this, RectTransform, DrivenTransformProperties.AnchoredPositionY | DrivenTransformProperties.SizeDeltaY);

            var worldBottom = _target.TransformPoint(new Vector3(0f, _target.rect.yMin, 0f));
            var bottom = parent.InverseTransformPoint(worldBottom).y - _extraBottom;

            var parentRect = parent.rect;
            var anchorY = parentRect.yMin + parentRect.height * RectTransform.anchorMin.y;

            var offsetMin = RectTransform.offsetMin;
            offsetMin.y = bottom - anchorY;
            RectTransform.offsetMin = offsetMin; 
        }
    }
}