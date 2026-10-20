using System;
using System.Collections.Generic;
using Sotbify.MobileApp.UI.Common.Components.Toggle.Interfaces;
using UnityEngine;

namespace Sotbify.MobileApp.UI.Common.Components.Toggle
{
    public abstract class BaseToggleGroup : MonoBehaviour, IToggleGroup
    {
        public abstract bool Register(IToggle toggle);
        public abstract bool Unregister(IToggle toggle);

        public abstract void SetIsOn<TState>(IToggle toggle, bool isOn, TState state = default, Action<bool, TState> callback = null);
    }
    
    public abstract class BaseToggleGroup<TToggle> : BaseToggleGroup, IToggleGroup where TToggle : IToggle
    {
        private readonly List<TToggle> _toggles = new();
        
        public IReadOnlyList<TToggle> Toggles => _toggles;
        
        public sealed override bool Register(IToggle toggle)
        {
            if (toggle is TToggle toggleButtonInternal)
                return Register(toggleButtonInternal);

            return false;
        }

        public bool Register(TToggle toggle)
        {
            if (_toggles.Contains(toggle))
                return false;
            
            _toggles.Add(toggle);
            return true;
        }
        
        public sealed override bool Unregister(IToggle toggle)
        {
            if (toggle is TToggle toggleButtonInternal)
                return Unregister(toggleButtonInternal);

            return false;
        }

        public bool Unregister(TToggle toggle)
        {
            if (_toggles.Contains(toggle) == false)
                return false;
            
            _toggles.Remove(toggle);
            return true;
        }
        
        public override void SetIsOn<TState>(IToggle toggle, bool isOn, TState state = default, Action<bool, TState> callback = null)
        {
            if(toggle is TToggle toggleButtonInternal)
                SetIsOn(toggleButtonInternal, isOn, state, callback);
        }

        public void SetIsOn<TState>(TToggle toggle, bool isOn, TState state = default, Action<bool, TState> callback = null)
        {
            if(toggle is null || _toggles.Contains(toggle) == false)
                return;
            
            SetIsOnInternal(toggle, isOn, state, callback);
        }
        
        protected abstract void SetIsOnInternal<TState>(TToggle toggle, bool isOn, TState state, Action<bool, TState> callback);
    }
}