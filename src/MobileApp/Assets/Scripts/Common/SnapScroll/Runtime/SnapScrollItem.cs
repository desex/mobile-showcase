using UnityEngine;
using UnityEngine.UI;

namespace Sotbifay.MobileApp.Common.SnapScroll
{
    public sealed class SnapScrollItem : MonoBehaviour
    {
        [SerializeField] private RectTransform _rootRectTransform;
        [SerializeField] private Button _button;
        [SerializeField] private SnapScrollView _scrollView;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if(_rootRectTransform == null)
                _rootRectTransform = transform as RectTransform;
            
            if (_scrollView == null)
                _scrollView = GetComponentInParent<SnapScrollView>();
        }
#endif

        private void Awake()
        {
            if (_button != null)
                _button.onClick.AddListener(OnClick);
        }

        private void OnDestroy()
        {
            if (_button != null)
                _button.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            if (_scrollView == null)
                return;

            var content = _scrollView.Content;
            if(content == null)
                return;

            for (var i = 0; i < content.childCount; i++)
            {
                var child = content.GetChild(i);
                if (child == null)
                    return;

                if (child == _rootRectTransform)
                {
                    _scrollView.SnapToIndex(i);
                    break;
                }
            }
        }
    }
}