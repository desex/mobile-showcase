using R3;
using Sotbify.MobileApp.Utils;

namespace Sotbify.MobileApp.Stab.GuideContentWindow
{
    public interface IGuideContent
    {
        public Observable<Unit> CloseObservable { get; }
        public Observable<EGuide> OpenContentObservable { get; }

        public void Close();
        public void OpenContent(EGuide guide);
    }
}