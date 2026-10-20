using R3;
using Sotbify.MobileApp.UI.Window.View;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.FAQWindow.Content
{
    public class FAQContentView : FadeWindowView
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private TMP_Text _content;
        [SerializeField] private TMP_Text _header;

        public Observable<Unit> BackButtonPressed => _backButton.OnClickAsObservable();

        public string Content { set => _content.text = value; }
        public string Header { set => _header.text = value; }
    }
}