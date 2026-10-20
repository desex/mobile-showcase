using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sotbify.MobileApp.UI.InputField
{
    public sealed class DragScrollInputField : TMP_InputField
    {
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private float _textSelectionDelay = 0.2f;

        private float _startTextSelectionTime;
        private bool _isTextSelection;
        private bool _shouldRestoreSelection;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (_scrollRect == null)
                _scrollRect = GetComponentInParent<ScrollRect>(true);
        }
#endif

        protected override void OnDisable()
        {
            _shouldRestoreSelection = false;
            base.OnDisable();
        }

        protected override void LateUpdate()
        {
            if (_shouldRestoreSelection)
            {
                _shouldRestoreSelection = false;

                var eventSystem = EventSystem.current;
                if (eventSystem != null && eventSystem.currentSelectedGameObject != gameObject)
                    eventSystem.SetSelectedGameObject(gameObject);
            }

            base.LateUpdate();
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            if (isFocused && IsPressedOnKeepFocusObject(eventData))
            {
                _shouldRestoreSelection = true;
                return;
            }

            base.OnDeselect(eventData);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            _startTextSelectionTime = Time.time;
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            var deltaBetweenDownAndDrag = Time.time - _startTextSelectionTime;
            if (deltaBetweenDownAndDrag >= _textSelectionDelay)
            {
                base.OnBeginDrag(eventData);
                _isTextSelection = true;
                return;
            }

            _isTextSelection = false;

            if (_scrollRect != null)
                _scrollRect.OnBeginDrag(eventData);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (_isTextSelection == false)
            {
                if (_scrollRect != null)
                    _scrollRect.OnDrag(eventData);
            }
            else
            {
                base.OnDrag(eventData);
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (_isTextSelection == false)
            {
                if (_scrollRect != null)
                    _scrollRect.OnEndDrag(eventData);
            }
            else
            {
                base.OnDrag(eventData);
            }

            _isTextSelection = false;
        }

        private static bool IsPressedOnKeepFocusObject(BaseEventData eventData)
        {
            if (eventData is not PointerEventData pointerEventData)
                return false;

            var pressedObject = pointerEventData.pointerCurrentRaycast.gameObject;
            return pressedObject != null && pressedObject.GetComponentInParent<KeepInputFieldFocus>() != null;
        }
    }
}