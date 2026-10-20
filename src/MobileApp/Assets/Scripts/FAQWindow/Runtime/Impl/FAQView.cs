using System.Collections.Generic;
using R3;
using Sotbify.MobileApp.UI.Window.View;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.FAQWindow
{
    public class FAQView : FadeWindowView
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _openChatButton;
        [SerializeField] private List<FAQItem> _items;
        
        public Observable<Unit> ClosePressed => _closeButton.OnClickAsObservable();
        public Observable<Unit> ChatButtonPressed => _openChatButton.OnClickAsObservable();
        public IReadOnlyList<FAQItem> Items => _items;
    }
}