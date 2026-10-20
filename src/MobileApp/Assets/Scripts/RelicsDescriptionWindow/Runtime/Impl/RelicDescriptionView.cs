using System;
using Coffee.UIEffects;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sotbify.MobileApp.UI.Window.View;

namespace Sotbify.MobileApp.RelicsDescriptionWindow
{
    public sealed class RelicDescriptionView : FadeWindowView
    {
        [SerializeField] private UIEffect _backgroundGradient;
        [Space]
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private RawImage _itemRawImage;
        [SerializeField] private TMP_Text _descriptionText;
        [Space]
        [SerializeField] private Button _closeButton;
        [Space]
        [SerializeField] private RectTransform _relicBackgroundContainer;
        [SerializeField] private RectTransform _relicContainer;
        
        public GradientColorKey[] GradientColorKeys
        {
            get
            {
                if(_backgroundGradient == null)
                    return Array.Empty<GradientColorKey>();
                
                _backgroundGradient.GetGradientKeys(out var colorKeys, out _, out _);
                return colorKeys;
            }

            set
            {
                if(_backgroundGradient == null)
                    return;
                
                _backgroundGradient.GetGradientKeys(out _, out var alphaKeys, out var gradientMode);
                _backgroundGradient.SetGradientKeys(value, alphaKeys, gradientMode);
            }
        }
        
        public GradientAlphaKey[] GradientAlphaKeys
        {
            get
            {
                if(_backgroundGradient == null)
                    return Array.Empty<GradientAlphaKey>();
                
                _backgroundGradient.GetGradientKeys(out _, out var alphaKeys, out _);
                return alphaKeys;
            }

            set
            {
                if(_backgroundGradient == null)
                    return;
                
                _backgroundGradient.GetGradientKeys(out var colorKeys, out var alphaKeys, out var gradientMode);
                _backgroundGradient.SetGradientKeys(colorKeys, value, gradientMode);
            }
        }

        public GradientMode GradientMode
        {
            get
            {
                if(_backgroundGradient == null)
                    return GradientMode.Blend;
                
                _backgroundGradient.GetGradientKeys(out _, out _, out var gradientMode);
                return gradientMode;
            }

            set
            {
                if(_backgroundGradient == null)
                    return;
                
                _backgroundGradient.GetGradientKeys(out var colorKeys, out var alphaKeys, out _);
                _backgroundGradient.SetGradientKeys(colorKeys, alphaKeys, value);
            }
        }
        
        public string Name
        {
            get => _nameText != null ? _nameText.text : string.Empty;
            set
            {
                if(_nameText != null)
                    _nameText.text = value;
            }
        }
        
        public Texture ItemTexture
        {
            get => _itemRawImage != null ? _itemRawImage.texture : null;
            set
            {
                if(_itemRawImage != null)
                    _itemRawImage.texture = value;
            }
        }
        
        public string Description
        {
            get => _descriptionText != null ? _descriptionText.text : string.Empty;
            set
            {
                if(_descriptionText != null)
                    _descriptionText.text = value;
            }
        }

        public Observable<Unit> CloseObservable => _closeButton != null 
         ? _closeButton.OnClickAsObservable()
         : Observable.Empty<Unit>();
    }
}