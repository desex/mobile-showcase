using System;
using System.Linq;
using LitMotion;
using LitMotion.Extensions;
using R3;
using Sotbify.MobileApp.Common.SerializableDictionary;
using Sotbify.MobileApp.Utils.Enums.Nexus;
using Sotbify.MobileApp.Utils.Enums.Relics;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.NexusWindow.Helmet
{
    public class HelmetView : MonoBehaviour
    {
        [SerializeField] private HelmetRelicHighlighter _helmetRelicHighlighter;
        [Space] 
        [SerializeField] private NexusCategoryCanvasGroup _helmetCategoryToVisual;

        [SerializeField] private HelmetCategoryToTutorialHighlighter _helmetCategoryToTutorialHighlighter;
        [SerializeField] private NexusCategoryButton _helmetCategoryToButton;
        [SerializeField] private NexusCategoryButton _dockCategoryToHelmetButton;
        [Space]
        [SerializeField] private float _animtionDuration = 1f / 3f;
        [SerializeField] private Ease _animtionEase = Ease.OutQuint;

        private ENexusCategory _currentCategory;
        private MotionHandle _motionHandle;

        public Observable<ENexusCategory> HelmetCategoryObservable => _helmetCategoryToButton.Where(static pair => pair.Value != null)
            .Select(static pair => pair.Value.OnClickAsObservable().Select(pair.Key, static (_, key) => key))
            .Merge();

        public Observable<ENexusCategory> DockCategoryObservable => _dockCategoryToHelmetButton.Where(static pair => pair.Value != null)
            .Select(static pair => pair.Value.OnClickAsObservable().Select(pair.Key, static (_, key) => key))
            .Merge();

        public void ChangeHelmet(ENexusCategory category)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        public void ChangeHighlight(ENexusCategory highlightCategory)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void UpdateHelmetRelicHighlighter(ENexusCategory category)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private MotionHandle? CreateFadeMotionHandle(CanvasGroup canvasGroup, bool isActive)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        [Serializable]
        private sealed class NexusCategoryCanvasGroup : SerializableDictionary<ENexusCategory, CanvasGroup> { }

        [Serializable]
        public sealed class HelmetCategoryToTutorialHighlighter : SerializableDictionary<ENexusCategory, GameObject> { }
        
        [Serializable]
        private sealed class NexusCategoryButton : SerializableDictionary<ENexusCategory, Button> { }
    }
}