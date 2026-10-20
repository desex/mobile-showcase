using R3;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Sotbify.MobileApp.PinCreationWindow
{
    public interface IPinCreationWindow
    {
        public bool IsChangingPin { get; }
        public UniTask SavePasswordAsync(string password, CancellationToken cancellationToken);
        public Observable<bool> ActionObservable { get; }
        public void SetChangingMode(bool isChangingPin);
        public void Close();
    }
}
