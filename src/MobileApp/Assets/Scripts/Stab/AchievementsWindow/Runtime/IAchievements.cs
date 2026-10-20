using R3;

namespace Sotbify.MobileApp.Stab.AchievementsWindow
{
    public interface IAchievements
    {
        public Observable<Unit> CloseObservable { get; }

        public void Close();
    }
}