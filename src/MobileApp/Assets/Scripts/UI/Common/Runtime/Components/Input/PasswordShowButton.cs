using System;
using R3;
using Sotbify.MobileApp.UI.Common.Components.Toggle.Implementations;
using TMPro;
using UnityEngine;

namespace Sotbify.MobileApp.UI.Common.Components.Input
{
    public sealed class PasswordShowButton : MonoBehaviour
    {
        [SerializeField] private ToggleButton _toggleButton;
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private TMP_InputField.ContentType _activeContentType = TMP_InputField.ContentType.Standard;

        private IDisposable _disposable;

        private void Awake()
        {
            _toggleButton.IsOn
                .Subscribe(this,
                    (isOn, button) =>
                    {
                        button._inputField.contentType =
                            isOn ? button._activeContentType : TMP_InputField.ContentType.Password;
                        button._inputField.ForceLabelUpdate();
                    })
                .AddTo(this);
        }
    }
}