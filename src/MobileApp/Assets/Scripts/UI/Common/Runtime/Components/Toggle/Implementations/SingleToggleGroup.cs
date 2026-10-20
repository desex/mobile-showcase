using System;
using System.Runtime.CompilerServices;
using R3;
using Sotbify.MobileApp.UI.Common.Components.Toggle.Interfaces;
using UnityEngine;

namespace Sotbify.MobileApp.UI.Common.Components.Toggle.Implementations
{
    [ExecuteAlways]
    public sealed class SingleToggleGroup : BaseToggleGroup<IToggle>
    {
        private const int DefaultIndex = -1;
        
        [SerializeField] private bool _allowSwitchOff = true;

        private readonly ReactiveProperty<int> _isOnIndex = new(DefaultIndex);
        private bool _isOnBusy = false;

        public bool AllowSwitchOff
        {
            get => _allowSwitchOff;
            set => _allowSwitchOff = value;
        }

        public ReadOnlyReactiveProperty<int> IsOnIndex => _isOnIndex;

        public void DeselectAll()
        {
            if(_allowSwitchOff == false)
                return;

            MakeOtherTogglesIsOff(null, out _);
            SetIsOnIndex(DefaultIndex);
        }

        protected override void SetIsOnInternal<TState>(IToggle toggle, bool isOn, TState state, Action<bool, TState> callback)
        {
            if (toggle is null)
            {
                callback?.Invoke(false, state);
                return;
            }

            var isOnPropertyValue = toggle.IsOn?.CurrentValue ?? false;
            if (isOnPropertyValue == isOn)
            {
                callback?.Invoke(false, state);
                return;
            }

            if (isOn == false)
            {
                if (_allowSwitchOff)
                {
                    callback?.Invoke(true, state);
                    SetIsOnIndex(DefaultIndex);
                }
                else
                {
                    callback?.Invoke(false, state);
                }
                return;
            }

            MakeOtherTogglesIsOff(toggle, out var toggleIndex);
            callback?.Invoke(toggleIndex >= 0, state);
            SetIsOnIndex(toggleIndex);
        }

        private void Awake() => 
            _isOnIndex.AddTo(this);

        private void MakeOtherTogglesIsOff(IToggle toggle, out int toggleIndex)
        {
            toggleIndex = DefaultIndex;
            try
            {
                SetIsOnBusy(true);
                for (var i = 0; i < Toggles.Count; i++)
                {
                    var toggleIternal = Toggles[i];
                    if (toggleIternal is null)
                        continue;

                    if (ReferenceEquals(toggleIternal, toggle))
                    {
                        toggleIndex = i;
                        continue;
                    }

                    toggleIternal.SetIsOn(false);
                }
            }
            finally
            {
                SetIsOnBusy(false);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetIsOnBusy(bool isBusy) => 
            _isOnBusy = isBusy;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetIsOnIndex(int index)
        {
            if (_isOnBusy == false)
                _isOnIndex.Value = index;
        }
    }
}