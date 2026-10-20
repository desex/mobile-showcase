using System;
using System.Collections.Generic;
using Coffee.UIEffects;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.RelicListWindow.Adapter
{
    public sealed class RelicView : MonoBehaviour
    {
        [SerializeField] private UIEffect _backgroundGradient;
        [Space]
        [SerializeField] private TMP_Text _name;
        [SerializeField] private Button _openDescriptionButton;
        [Space]
        [SerializeField] private int _rarity = 0;
        [SerializeField] private List<Graphic> _rarityItems;
        [SerializeField] private Color _rarityActiveColor = Color.white; 
        [SerializeField] private Color _rarityInactiveColor = new(1f, 1f, 1f, 0.5f); 
        [Space]
        [SerializeField] private RawImage _itemRawImage;
        
        public int Id { get; set; } = -1;

        public string Name
        {
            get => _name != null ? _name.text : string.Empty;
            set
            {
                if(_name != null)
                    _name.text = value;
            }
        }

        public int Rarity
        {
            get => _rarity;
            set
            {
                _rarity = value;
                UpdateRarityItems(value);
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

        public Observable<int> OpenDescription => _openDescriptionButton
            .OnClickAsObservable()
            .Select(this, static (_, @this) => @this.Id);

#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateRarityItems(_rarity);
        }
#endif

        private void Awake()
        {
            UpdateRarityItems(_rarity);
        }

        private void UpdateRarityItems(int rarity)
        {
            for (var i = 0; i < _rarityItems.Count; i++)
            {
                var rarityItem = _rarityItems[i];
                if(rarityItem == null)
                    continue;
                
                rarityItem.color = i <= rarity - 1
                    ? _rarityActiveColor 
                    : _rarityInactiveColor;
            }
        }
    }
}