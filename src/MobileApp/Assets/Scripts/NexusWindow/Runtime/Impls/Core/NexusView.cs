using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.NexusWindow.Dock;
using Sotbify.MobileApp.NexusWindow.Helmet;
using Sotbify.MobileApp.UI.Window.View;
using UnityEngine;

namespace Sotbify.MobileApp.NexusWindow.Core
{
    public class NexusView : FadeWindowView
    {
        [SerializeField] private DockView _dockView;
        [SerializeField] private HelmetView _helmet;
        
        public DockView DockView => _dockView;
        public HelmetView Helmet => _helmet;
        
        protected override UniTask ShowInternalAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}