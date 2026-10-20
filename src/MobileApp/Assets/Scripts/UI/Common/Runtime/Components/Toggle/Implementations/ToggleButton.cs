using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.UI.Common.Components.Toggle.Implementations
{
    public sealed class ToggleButton : BaseToggle
    {
        [SerializeField] private Button _button;

        protected override void OnAwakeInternal()
        {
            if(_button == null)
                return;

            _button
                .OnClickAsObservable()
                .Subscribe(this, (_, state) =>
                {
                    var isOnProperty = state.IsOn;
                    state.SetIsOn(isOnProperty.CurrentValue == false);
                })
                .AddTo(this);
        }
    }
}