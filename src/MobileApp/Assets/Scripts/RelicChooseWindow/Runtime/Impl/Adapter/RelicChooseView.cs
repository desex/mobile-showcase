using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Animation;
using R3;
using Sirenix.OdinInspector;
using Sotbify.MobileApp.Utils;
using Sotbify.MobileApp.Utils.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace Sotbify.MobileApp.RelicChooseWindow.Adapter
{
    public class RelicChooseView : MonoBehaviour
    {
        private const string RarityGroup = "Rarity";
        
        [BoxGroup(RarityGroup)]
        [SerializeField] private int _rarity = 0;
        [BoxGroup(RarityGroup)]
        [SerializeField] private List<Graphic> _stars = new();
        [BoxGroup(RarityGroup)]
        [SerializeField] private Color _activeStarColor = Color.white;
        [BoxGroup(RarityGroup)]
        [SerializeField] private Color _inativeStarColor = new (1f, 1f, 1f, 0.5f);
        
        [SerializeField] private TMP_Text _relicName;
        [SerializeField] private Button _openSmallDescriptionButton;
        
        [SerializeField] private RawImage _artifactPreview;
        [SerializeField] private LitMotionAnimation _loadingAnimation;
        [SerializeField] private Graphic _chosenMark;
        [SerializeField] private CanvasGroup[] _outlineGroups;
        [SerializeField] private MotionParameters _chooseMotionParameters;
        
        private int _id;
        private bool _isChosen;

        public Texture ItemPreviewTexture
        {
            get => _artifactPreview != null ? _artifactPreview.texture : null;
            set
            {
                if (_artifactPreview != null)
                    _artifactPreview.texture = value;
            }
        }
        
        public string RelicName { set => _relicName.text = value; }
        public int Id { get => _id; set => _id = value; }

        public int Rarity
        {
            get => _rarity;
            set
            {
                _rarity = value;
                UpdateRarityStars(value);
            }
        }

        public void ChangeEquipStatus(bool isEquiped) => 
            _chosenMark.gameObject.SetActive(isEquiped);

        public Observable<int> OpenSmallDescriptionObservable => _openSmallDescriptionButton
            .OnClickAsObservable()
            .Select(_ => _id);

        public void StartLoadingAnimation()
        {
            _loadingAnimation.gameObject.SetActive(true);
            _loadingAnimation.Restart();
        }

        public void StopLoadingAnimation()
        {
            _loadingAnimation.Stop();
            _loadingAnimation.gameObject.SetActive(false);
        }
        
        public async UniTask SetSelectedAsync(bool isSelected, CancellationToken cancellationToken = default)
        {
            if (_isChosen == isSelected)
                return;

            _isChosen = isSelected;
            using (ListPool<UniTask>.Get(out var tasks))
            {
                foreach (var outlineGroup in _outlineGroups)
                    tasks.Add(outlineGroup.CreateOpacityTask(
                        outlineGroup.alpha, isSelected ? 1f : 0f,
                        _chooseMotionParameters,
                        CancelBehavior.Complete,
                        cancellationToken
                        ));
                
                await UniTask.WhenAll(tasks);
            }
        }

        public void SetSelectedInstantly(bool isSelected)
        {
            _isChosen = isSelected;
            foreach (var outlineGroup in _outlineGroups)
                outlineGroup.alpha = isSelected ? 1f : 0f;
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
        }

        private void UpdateRarityStars(int rarity)
        {
            for (var i = 0; i < _stars.Count; i++)
            {
                var star = _stars[i];
                if(star == null)
                    continue;
                
                star.color = i <= rarity - 1 ? _activeStarColor : _inativeStarColor;
            }
        }
    }
}