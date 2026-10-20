using System.Collections.Generic;
using System.Linq;
using R3;
using Sotbify.MobileApp.UI.Window.View;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.AllRelicsWindow
{
    public sealed class AllRelicsWindowView : FadeWindowView
    {
        [SerializeField] private List<Button> _buttons;

        public Observable<Unit> CloseObservable => _buttons.Where(static button => button != null)
            .Select(static button => button.OnClickAsObservable())
            .Merge();
    }
}