using UnityEngine;

namespace Sotbify.MobileApp.UI.Common.Components.UIGradient
{
    public interface IUIGradient
    {
        public (Color, Color) Gradient { get; }
        public void SetGradient(Color from, Color to);
    }
}