using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.RelicListWindow.Adapter;
using Sotbify.MobileApp.UI.Window.View;
using TMPro;
using UnityEngine;

namespace Sotbify.MobileApp.RelicListWindow
{
    public class RelicListWindowView : FadeWindowView
    {
        [SerializeField] private TMP_Text _timerText;
        
        [field: SerializeField] public int PrewarmCount { get; private set; }
        [field: SerializeField] public RelicsListView ListView { get; private set; }
        
        public Observable<int> OpenRelicDescriptionObservable => ListView.OpenDescriptionObservable;
        
        protected override UniTask ShowInternalAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void ChangeTimer(TimeSpan timeSpan)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}