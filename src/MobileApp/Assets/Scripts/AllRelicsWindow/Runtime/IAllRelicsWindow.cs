using R3;

namespace Sotbify.MobileApp.AllRelicsWindow
{
    public interface IAllRelicsWindow
    {
        bool WasShowed { get; }
        Observable<Unit> CloseObservable { get; }

        void MakeShowed();
        void Close();
    }
}