using R3;

namespace Sotbify.MobileApp.UI.Common.Components.Toggle.Interfaces
{
    public interface IToggle
    {
        ReadOnlyReactiveProperty<bool> IsOn { get; }
        
        void SetIsOn(bool isOn);
    }
}