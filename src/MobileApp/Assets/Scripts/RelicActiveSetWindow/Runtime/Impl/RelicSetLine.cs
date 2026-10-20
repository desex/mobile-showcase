using System.Collections.Generic;
using R3;
using Sirenix.OdinInspector;
using TierStudios.UIParticleSystem.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.RelicActiveSetWindow
{
    public sealed class RelicSetLine : MonoBehaviour
    {
        private const string VfxGroup = "VFX";
        private static readonly Gradient EmptyGradient = new();

        [SerializeField] private TMP_Text _relicName;
        [Space] 
        [SerializeField] private int _rarity = 0;
        [SerializeField] private List<Graphic> _stars;
        [SerializeField] private Color _activeStarColor = Color.white;
        [SerializeField] private Color _inativeStarColor = new(1f, 1f, 1f, 0.5f);
        [Space] 
        [SerializeField] private RawImage _preview;
        [SerializeField] private Texture2D _emptyTexture;
        [SerializeField] private Button _editRelicButton;
        [SerializeField] private GameObject _activeView;
        [SerializeField] private GameObject _disabledView;

        [BoxGroup(VfxGroup)] [SerializeField] private GameObject _vfxRoot;
        [BoxGroup(VfxGroup)] [SerializeField] private UIParticleSystem _glowParticleSystem;
        [BoxGroup(VfxGroup)] [SerializeField] private UIParticleSystem _rayParticleSystem;
        [BoxGroup(VfxGroup)] [SerializeField] private UIParticleSystem _particlesParticleSystem;

        public Observable<int> EditRelicObservable => _editRelicButton.OnClickAsObservable().Select(_ => Id);

        public int Id { get; set; }

        public string Name
        {
            get => _relicName != null ? _relicName.text : string.Empty;
            set
            {
                if (_relicName != null)
                    _relicName.text = value;
            }
        }

        public Texture Preview
        {
            get => _preview != null ? _preview.texture : null;
            set
            {
                if (_preview == null)
                    return;

                _preview.texture = value != null ? value : _emptyTexture;
            }
        }

        public int Rarity
        {
            get => _rarity;
            set
            {
                _rarity = value;
                UpdateRarityStars(value);
            }
        }

        public bool VfxIsActive
        {
            get => _vfxRoot != null && _vfxRoot.activeSelf;
            set
            {
                if (_vfxRoot != null)
                    _vfxRoot.SetActive(value);

                SetVfxEmitting(value);
            }
        }

        public Gradient GlowOverLifetime
        {
            get => GetGradientOverLifetime(_glowParticleSystem);
            set => SetGradientOverLifetime(_glowParticleSystem, value);
        }

        public Gradient RayOverLifetime
        {
            get => GetGradientOverLifetime(_rayParticleSystem);
            set => SetGradientOverLifetime(_rayParticleSystem, value);
        }

        public Gradient ParticlesOverLifetime
        {
            get => GetGradientOverLifetime(_particlesParticleSystem);
            set => SetGradientOverLifetime(_particlesParticleSystem, value);
        }

        public void ChangeState(bool isActive)
        {
            _activeView.SetActive(isActive);
            _disabledView.SetActive(isActive == false);

            if (isActive && VfxIsActive)
                SetVfxEmitting(true);
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            UpdateRarityStars(_rarity);
        }

#endif

        private void Awake()
        {
            UpdateRarityStars(_rarity);

            if (_preview != null && _preview.texture == null)
                _preview.texture = _emptyTexture;
        }

        private void SetVfxEmitting(bool emitting)
        {
            SetEmitting(_glowParticleSystem, emitting);
            SetEmitting(_rayParticleSystem, emitting);
            SetEmitting(_particlesParticleSystem, emitting);
        }

        private static void SetEmitting(UIParticleSystem uiParticleSystem, bool emitting)
        {
            if (uiParticleSystem == null)
                return;

            if (emitting)
            {
                uiParticleSystem.StartEmitting();
            }
            else
            {
                uiParticleSystem.StopEmitting();
            }
        }

        private static Gradient GetGradientOverLifetime(UIParticleSystem uiParticleSystem)
        {
            if (uiParticleSystem == null)
                return EmptyGradient;

            var colorSettings = uiParticleSystem.ColorSettings;
            return colorSettings.colorGradient;
        }

        private static void SetGradientOverLifetime(UIParticleSystem uiParticleSystem, Gradient gradient)
        {
            if (uiParticleSystem == null)
                return;

            // Костыль для обновления цвета, внутри плагина ошибка какая-то
            uiParticleSystem.StopEmitting();
            uiParticleSystem.gameObject.SetActive(false);

            var colorSettings = uiParticleSystem.ColorSettings;
            colorSettings.colorGradient = gradient ?? EmptyGradient;

            // Костыль для обновления цвета, внутри плагина ошибка какая-то
            uiParticleSystem.gameObject.SetActive(true);
            uiParticleSystem.StartEmitting();
        }

        private void UpdateRarityStars(int rarity)
        {
            for (var i = 0; i < _stars.Count; i++)
            {
                var star = _stars[i];
                if (star == null)
                    continue;

                star.color = i <= rarity - 1 ? _activeStarColor : _inativeStarColor;
            }
        }
    }
}