using System;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.UI.Window.Presenter;
using Sotbify.MobileApp.Utils.Enums.Nexus;

namespace Sotbify.MobileApp.NexusWindow.Core
{
    public class NexusPresenter : WindowPresenter<INexus, NexusView>
    {
        private IDisposable _bindings;

        public override void Initialize()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override void OnDispose()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}