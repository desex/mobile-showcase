using System;
using R3;
using Sotbify.MobileApp.Utils;

namespace Sotbify.MobileApp.Stab.GuideContentWindow
{
    public class GuideContent : IGuideContent, IDisposable
    {
        private readonly Subject<Unit> _close = new();
        private readonly Subject<EGuide> _openContent = new();

        public Observable<Unit> CloseObservable => _close;
        public Observable<EGuide> OpenContentObservable => _openContent;

        public void Close()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void OpenContent(EGuide guide)
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