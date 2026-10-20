using System;
using R3;
using Sotbify.MobileApp.PinPassword;

namespace Sotbify.MobileApp.TwoFactorWindow
{
    public class TwoFactorWindow : ITwoFactorWindow, IDisposable
    {
        private readonly Subject<Unit> _closeWindow = new();
        private readonly Subject<Unit> _changePin = new();
        private readonly IPinPasswordService _pinPasswordService;

        public Observable<Unit> CloseWindowObservable => _closeWindow;
        public Observable<Unit> ChangePinObservable => _changePin;

        public TwoFactorWindow(IPinPasswordService pinPasswordService)
        {
            _pinPasswordService = pinPasswordService;
        }

        public void Close()
            {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void ChangePin()
            {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void DisablePassword()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void Dispose()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}