using R3;

namespace Sotbify.MobileApp.GreetingsWindow
{
    public class GreetingsWindow : IGreetingsWindow
    {
        private readonly Subject<Unit> _scanButtonPressed = new ();
        private readonly Subject<Unit> _openPolicyButtonPressed = new ();

        public Observable<Unit> ScanButtonPressed => _scanButtonPressed;
        public Observable<Unit> OpenPolicyPressed => _openPolicyButtonPressed;
        void IGreetingsWindow.OnScanButtonPressed()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void OpenPolicy()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}