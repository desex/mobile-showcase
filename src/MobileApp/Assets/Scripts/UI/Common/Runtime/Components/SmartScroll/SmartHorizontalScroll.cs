using UnityEngine;
using UnityEngine.EventSystems;

namespace Sotbify.MobileApp.UI.Common.Components.SmartScroll
{
    public class SmartHorizontalScroll : UnityEngine.UI.ScrollRect
    {
        private bool _shouldParentHandle = false;

        public override void OnInitializePotentialDrag(PointerEventData eventData)
        {
            base.OnInitializePotentialDrag(eventData);
            ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.initializePotentialDrag);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            var deltaX = Mathf.Abs(eventData.delta.x);
            var deltaY = Mathf.Abs(eventData.delta.y);
            
            if (!_shouldParentHandle && horizontal &&  deltaX > deltaY)
            {
                base.OnDrag(eventData);
            }
            else
            {
                _shouldParentHandle = true;
                ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.dragHandler);
            }
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            var deltaX = Mathf.Abs(eventData.delta.x);
            var deltaY = Mathf.Abs(eventData.delta.y);
            
            _shouldParentHandle = deltaY > deltaX;

            if (_shouldParentHandle)
            {
                ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.beginDragHandler);
                return;
            }
            
            base.OnBeginDrag(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (_shouldParentHandle)
                ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.endDragHandler);
            else
                base.OnEndDrag(eventData);

            _shouldParentHandle = false;
        }
    }
}