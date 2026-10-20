using System;
using System.Collections.Generic;
using R3;
using Sotbify.MobileApp.Common.SerializableDictionary;
using Sotbify.MobileApp.UI.Window.View;
using UnityEngine;
using UnityEngine.UI;

namespace MainPolicyWindow
{
    public class MainPolicyView : FadeWindowView
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private UrlToButton _urlToButton;

        public IReadOnlyDictionary<string, Button> UrlToButton => _urlToButton;
        public Observable<Unit> CloseButtonPressed => _closeButton.OnClickAsObservable();
    }

    [Serializable]
    public sealed class UrlToButton : SerializableDictionary<string, Button>
    {
    }
}