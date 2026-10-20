using R3;

namespace Sotbify.MobileApp.HiddenModeWindow
{
    public interface IHiddenModeWindow
    {
        Observable<bool> CreatePinObservable { get; }
        void Close();
        void CreatePin();
    }
}
