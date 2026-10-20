using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.NexusWindow.Dock;
using Sotbify.MobileApp.UI.Window.Presenter;
using Sotbify.MobileApp.Utils.Enums;
using Sotbify.MobileApp.Utils.Enums.Nexus;

namespace Sotbify.MobileApp.Stab.StabNexus.Core
{
    public class StabNexusPresenter : WindowPresenter<IStabNexus, StabNexusView>
    {
        private readonly CompositeDisposable _disposables = new ();
        
        public override void Initialize()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private void SubscribeItem(KeyValuePair<ENexusCategory, DockItem> keyItemPair)
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