using R3;

namespace Sotbify.MobileApp.TwoFactorWindow
{
    public interface ITwoFactorWindow
    {
        public Observable<Unit> CloseWindowObservable { get; }
        public Observable<Unit> ChangePinObservable { get; }
        public void Close();
        public void ChangePin();
        public void DisablePassword();
    }
}
