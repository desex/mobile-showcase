using System;

namespace Sotbify.MobileApp.UI.Common.Components.Toggle.Interfaces
{
    public interface IToggleGroup
    {
        bool Register(IToggle toggle);
        bool Unregister(IToggle toggle);

        void SetIsOn<TState>(IToggle toggle, bool isOn, TState state = default, Action<bool, TState> callback = null);
    }

    public interface IToggleGroup<in TToggle> : IToggleGroup
        where TToggle : IToggle
    {
        bool Register(TToggle toggle);
        bool Unregister(TToggle toggle);
        
        void SetIsOn<TState>(TToggle toggle, bool isOn, TState state = default, Action<bool, TState> callback = null);
    }
}