using R3;
using Sotbify.MobileApp.UI.Window.Presenter;
using UnityEngine;

namespace Sotbify.MobileApp.RelicsDescriptionWindow
{
    public class RelicDescriptionPresenter : WindowPresenter<IRelicDescription, RelicDescriptionView>
    {
        private readonly CompositeDisposable _disposables = new();

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

        private static GradientColorKey[] UpdateGradientColors
        (
            GradientColorKey[] keys,
            Color gradientFromColor,
            Color mainColor,
            Color gradientToColor
        )
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}