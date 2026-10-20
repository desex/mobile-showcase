using System;
using R3;
using Sotbify.MobileApp.Common.SerializableDictionary;
using Sotbify.MobileApp.UI.Window.View;
using Sotbify.MobileApp.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.Stab.GuideContentWindow
{
    public class GuideContentView : FadeWindowView
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private EGuideToObject _guideObjects;

        public Observable<Unit> CloseButtonObservable => _closeButton.OnClickAsObservable();

        public void OpenGuide(EGuide guide)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }

    [Serializable]
    public sealed class EGuideToObject : SerializableDictionary<EGuide, GameObject>
    {
    }
}