using System;
using R3;
using Sotbify.MobileApp.Common.Preferences;

namespace Sotbify.MobileApp.AllRelicsWindow
{
    public sealed class AllRelicsWindow : IAllRelicsWindow, IDisposable
    {
        private const string WasShowedKey = nameof(AllRelicsWindow) + "_WasShowed";
        
        private readonly Subject<Unit> _closeSubject = new();
        
        private readonly IPreferences _preferences;

        public AllRelicsWindow(IPreferences preferences)
        {
            _preferences = preferences ?? throw new ArgumentNullException(nameof(preferences));
        }

        public bool WasShowed => _preferences.GetBool(WasShowedKey);
        public Observable<Unit> CloseObservable => _closeSubject;

        public void MakeShowed() {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void Close() {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void Dispose() {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}