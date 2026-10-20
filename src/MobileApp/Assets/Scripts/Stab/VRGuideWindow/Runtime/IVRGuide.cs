using R3;
using Sotbify.MobileApp.Utils;

namespace Sotbify.MobileApp.Stab.VRGuideWindow
{
    public interface IVRGuide
    {
        public Observable<EGuide> OpenGuideObservable { get; }
        public Observable<Unit> CloseObservable { get; }

        public void Close();
        public void OpenGuide(EGuide guide);
    }
}