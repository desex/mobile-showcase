using System;
using R3;

namespace Sotbify.MobileApp.DeletePopupWindow
{
    public class DeletePopup : IDeletePopup, IDisposable
    {
        private readonly Subject<Unit> _deleteSubject = new();
        private readonly Subject<Unit> _closeSubject = new();
        public Observable<Unit> OnDeletePressed => _deleteSubject;
        public Observable<Unit> OnClosePressed => _closeSubject;
        
        public void Delete()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

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