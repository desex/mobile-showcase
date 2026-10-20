using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.UI.Window.Presenter;

namespace Sotbify.MobileApp.AllRelicsWindow
{
    public sealed class AllRelicsWindowPresenter : WindowPresenter<IAllRelicsWindow, AllRelicsWindowView>
    {
        private IDisposable _bindings;

        public override void Initialize()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override UniTask BeforeShowAsync(CancellationToken cancellationToken)
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