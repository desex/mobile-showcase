using R3;

namespace Sotbify.MobileApp.Stab.SettingsWindow
{
    public interface ISettings
    {
        public Observable<Unit> CloseObservable { get; }

        public void Close();
    }
}