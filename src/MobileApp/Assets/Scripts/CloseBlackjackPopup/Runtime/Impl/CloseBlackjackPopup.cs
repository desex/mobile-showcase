using System;
using R3;

namespace Sotbify.MobileApp.CloseBlackjackPopup
{
    public class CloseBlackjackPopup : ICloseBlackjackPopup, IDisposable
    {
        private readonly Subject<Unit> _confirmSubject = new();
        private readonly Subject<Unit> _cancelSubject = new();

        public Observable<Unit> OnContinuePressed => _confirmSubject;
        public Observable<Unit> OnCancelPressed => _cancelSubject;

        public void Continue()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void Cancel()
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