using System;
using R3;

namespace Sotbify.MobileApp.Stab.SettingsWindow
{
    public class Settings : ISettings, IDisposable
    {
        private readonly Subject<Unit> _close = new();

        public Observable<Unit> CloseObservable => _close;
        
        public void Close()
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