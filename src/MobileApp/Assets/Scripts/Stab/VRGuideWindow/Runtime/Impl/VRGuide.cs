using R3;
using Sotbify.MobileApp.Utils;

namespace Sotbify.MobileApp.Stab.VRGuideWindow
{
    public class VRGuide : IVRGuide
    {
        private readonly Subject<EGuide> _openGuide = new();
        private readonly Subject<Unit> _close = new();
        public Observable<EGuide> OpenGuideObservable => _openGuide;
        public Observable<Unit> CloseObservable => _close;
        
        public void Close()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        public void OpenGuide(EGuide guide)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}