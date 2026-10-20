using R3;

namespace Sotbify.MobileApp.HiddenModeWindow
{
    public class HiddenModeWindow : IHiddenModeWindow
    {
        private readonly Subject<bool> _createPin = new();

        public Observable<bool> CreatePinObservable => _createPin;

        public void Close() {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void CreatePin()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}
