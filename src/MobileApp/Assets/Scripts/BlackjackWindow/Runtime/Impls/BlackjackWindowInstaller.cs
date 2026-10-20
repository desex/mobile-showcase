using System;
using Sotbify.MobileApp.BlackjackWindow.Core;
using Sotbify.MobileApp.UI.Window;
using Sotbify.MobileApp.Utils.Data.Blackjack;
using Sotbify.MobileApp.Utils.Extensions.Blackjack;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Sotbify.MobileApp.BlackjackWindow
{
    [Serializable]
    public class BlackjackWindowInstaller : IInstaller
    {
        [SerializeField] private CardView _cardPrefab;

        public void Install(IContainerBuilder builder)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}
