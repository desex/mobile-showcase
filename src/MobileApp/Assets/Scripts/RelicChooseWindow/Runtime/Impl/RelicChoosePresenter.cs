using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.Relic;
using Sotbify.MobileApp.RelicsDescriptionWindow;
using Sotbify.MobileApp.UI.Window.Presenter;
using UnityEngine;

namespace Sotbify.MobileApp.RelicChooseWindow
{
    public sealed class RelicChoosePresenter : WindowPresenter<IRelicChooseModel, RelicChooseWindowView>
    {
        private IDisposable _bindings;
        private IDisposable _descriptionBindings;

        public override void Initialize()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override async UniTask AfterShowAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override UniTask BeforeHideAsync(CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        protected override void OnDispose()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        private async UniTask UpdateRelicDescriptionAsync(RelicDescriptionModel relic, CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}