using System;
using System.Collections.Generic;
using R3;
using Sotbify.MobileApp.Common.SerializableDictionary;
using Sotbify.MobileApp.UI.Window.View;
using Sotbify.MobileApp.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.Stab.VRGuideWindow
{
    public class VRGuideView : FadeWindowView
    {
        [SerializeField] private EGuideToButton _guideToButton;
        [SerializeField] private Button _closeButton;

        public Observable<Unit> CloseButtonObservable => _closeButton.OnClickAsObservable();
        public IReadOnlyDictionary<EGuide, Button> EGuideToButtons => _guideToButton;
    }
    
    [Serializable]
    public sealed class EGuideToButton : SerializableDictionary<EGuide, Button>
    {
    }
}