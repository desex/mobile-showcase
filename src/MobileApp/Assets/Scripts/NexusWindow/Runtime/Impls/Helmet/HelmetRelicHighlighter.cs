using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using R3;
using Sotbify.MobileApp.Utils;
using Sotbify.MobileApp.Utils.Enums.Relics;
using Sotbify.MobileApp.Utils.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.NexusWindow.Helmet
{
    public class HelmetRelicHighlighter : MonoBehaviour
    {
        [SerializeField] private RectTransform _highlighter;
        [SerializeField] private TMP_Text _collectionsText;
        [SerializeField] private TMP_Text _activeSetText;
        [SerializeField] private Button _collectionsButton;
        [SerializeField] private Button _activeSetButton;
        [SerializeField] private MotionParameters _motionParameters;
        
        private EActiveCategory _activeCategory;
        private IDisposable _bindings;
        
        private Observable<Unit> CollectionsObservable => _collectionsButton.OnClickAsObservable();
        private Observable<Unit> ActiveSetObservable => _activeSetButton.OnClickAsObservable();
        
        private RectTransform CollectionsRectTransform => _collectionsButton.transform.AsRectTransform();
        private RectTransform ActiveSetRectTransform => _activeSetButton.transform.AsRectTransform();
        
        public void SetActiveWithoutAnimation(EActiveCategory targetActive)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void OnEnable()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void OnDisable()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        private UniTask MoveTo(EActiveCategory targetActive, CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}