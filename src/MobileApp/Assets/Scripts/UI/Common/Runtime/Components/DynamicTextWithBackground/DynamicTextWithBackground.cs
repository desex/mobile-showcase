using Sotbify.MobileApp.Utils.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.UI.Common.Components.DynamicTextWithBackground
{
    public class DynamicTextWithBackground : MonoBehaviour
    {
        [SerializeField] private TMP_Text _message;
        [SerializeField] private Graphic[] _colorChangeGraphics;
        [SerializeField] private RectTransform _containerRectTransform;

        [Range(0f, 1f)]
        [SerializeField] 
        private float _maxWidthRatio;
        
        [SerializeField] private float _canvasWidth;
        [SerializeField] private float _minWidth;
        [SerializeField] private float _minHeight;
        [SerializeField] private Vector2 _verticalTextMargins = new (12.5f, 12.5f);

        private float MaxWidth => _canvasWidth * _maxWidthRatio;
        private RectTransform RectTransform => transform.AsRectTransform();

        public string Message
        {
            get => _message.text;
            set => _message.SetText(value);
        }
        
        public Color TextColor { set => _message.color = value; }
        
        public Color Color
        {
            get
            {
                foreach (var colorChangeGraphic in _colorChangeGraphics)
                {
                    if (colorChangeGraphic != null)
                        return colorChangeGraphic.color;
                }
                
                return Color.clear;
            }

            set
            {
                foreach (var graphic in _colorChangeGraphics)
                {
                    var alpha = graphic.color.a;
                    graphic.color = value.ChangeAlpha(alpha);
                }
            }
        }

        public Vector2 ApplyPreferredSize()
        {
            var preferredWidth = _message.GetPreferredValues(_message.text, float.PositiveInfinity, 0).x;
            preferredWidth = Mathf.Clamp(preferredWidth, _minWidth, MaxWidth);
            
            _containerRectTransform.sizeDelta = new Vector2(preferredWidth, _containerRectTransform.sizeDelta.y);
            
            _message.ForceMeshUpdate(true, true);
            
            var linesCount = _message.textInfo.lineCount;
            var lineHeight = _message.textInfo.lineInfo[0].lineHeight;
            var preferredHeight = lineHeight * linesCount;
            
            preferredHeight += _verticalTextMargins.x + _verticalTextMargins.y;
            preferredHeight = Mathf.Max(preferredHeight, _minHeight);
            
            var preferredSize = new Vector2(preferredWidth, preferredHeight);
            ApplyPreferredSize(preferredSize);
            return preferredSize;
        }

        public void ApplyPreferredSize(Vector2 preferredSize)
        {
            RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, preferredSize.y);
            _containerRectTransform.sizeDelta = preferredSize;
        }
    }
}