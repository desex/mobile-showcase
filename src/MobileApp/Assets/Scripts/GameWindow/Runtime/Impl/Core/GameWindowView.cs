using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbify.MobileApp.PlayWindow.Adapter;
using Sotbify.MobileApp.UI.Window.View;
using UnityEngine;

namespace Sotbify.MobileApp.PlayWindow.Core
{
    public class GameWindowView : FadeWindowView
    {
        [field: SerializeField] public GameListView ListView { get; private set; }
        
        protected override UniTask ShowInternalAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}