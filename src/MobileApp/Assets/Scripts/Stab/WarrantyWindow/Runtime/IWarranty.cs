using R3;

namespace Stab.WarrantyWindow
{
    public interface IWarranty
    {
        public Observable<Unit> CloseObservable { get; }
        public void GoChat();
        public void Close();
    }
}