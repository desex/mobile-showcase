using R3;

namespace Sotbify.MobileApp.GreetingsWindow
{
    public interface IGreetingsWindow
    {
        public Observable<Unit> ScanButtonPressed { get; }
        public Observable<Unit> OpenPolicyPressed { get; }
        public void OnScanButtonPressed();
        public void OpenPolicy();
    }
}