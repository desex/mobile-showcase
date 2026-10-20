using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using R3;
using Sotbify.MobileApp.UI.Window.View;
using Sotbify.MobileApp.Utils;
using Sotbify.MobileApp.Utils.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sotbify.MobileApp.CheckPasswordWindow
{
    public class CheckPasswordWindowView : FadeWindowView
    {
        [SerializeField] private TMP_InputField _pinInput;
        [SerializeField] private Button _forgetPinButton;
        [SerializeField] private TMP_Text[] _pinDigits;
        [SerializeField] private GameObject[] _outlines;
        [SerializeField] private GameObject[] _cursors;

        [SerializeField] private Graphic[] _colorDependecyGraphics;

        [SerializeField] private MotionParameters _incorrectMotion;
        [SerializeField] private Color _incorrectColor;
        [SerializeField] private Color _initialColor;
        [SerializeField] private float _horizontalSpread;
        [SerializeField] private float _dampingRatio = 1f;
        [SerializeField] private int _frequency = 12;

        public Observable<string> PinChangedObservable => _pinInput.onValueChanged.AsObservable();
        public Observable<Unit> ForgetPasswordObservable => _forgetPinButton.OnClickAsObservable();

        protected override void OnAwakeInternal()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void SetPin(string pin)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void FocusInput()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public async UniTask ShowIncorrectAnimation(CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}