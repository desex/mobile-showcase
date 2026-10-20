using Sotbify.MobileApp.Utils.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.ChatMessages.Views
{
    public class ManagerMessageView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _message;
        [SerializeField] private RectTransform _containerRectTransform;
        [Range(0f, 1f)]
        [SerializeField] 
        private float _maxWidthRatio;
        
        [SerializeField] private float _canvasWidth;
        [SerializeField] private float _minWidth;
        [SerializeField] private float _minHeight;
        [SerializeField] private Vector2 _verticalTextMargins = new (12.5f, 12.5f);
         
        [Header("Image")]
        [SerializeField] private Image _image;
        [SerializeField] private AspectRatioFitter _aspectRatioFitter;
        [SerializeField] private float _maxImageHeight;
        [SerializeField] private float _paddingLeft;
        [SerializeField] private float _paddingRight;
        [SerializeField] private float _paddingTop;
        [SerializeField] private float _paddingBottom;
        
        private Vector2 _originalSizes;
        private float MaxWidth => _canvasWidth * _maxWidthRatio;
        private RectTransform RectTransform => transform.AsRectTransform();
        

        public string Message
        {
            get => _message.text;
            set => _message.SetText(value);
        }
        
        public Sprite Sprite
        {
            get => _image.sprite;
            set
            {
                _image.gameObject.SetActive(value != null);
                _image.sprite = value;
                
                if (value == null)
                    return;

                _originalSizes = value.rect.size;
            }
        }

        public void ApplyPreferredSize(Vector2 preferredValues)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        public Vector2 ApplyPreferredSize()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private Vector2 ApplyImageSize()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void ApplyImageSize(Vector2 preferredValues)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private Vector2 CalculateImageSize()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}