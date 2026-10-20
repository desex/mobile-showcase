using System.Threading;
using Cysharp.Threading.Tasks;
using R3;

namespace Sotbify.MobileApp.CheckPasswordWindow
{
    public interface ICheckPasswordWindow
    {
        public bool IsChangingPin { get; }
        public Observable<bool> IsPasswordValid { get; }
        public Observable<Unit> ForgotPasswordObservable { get; }
        public void SetChangingMode(bool isChangingPin);
        public UniTask CheckPasswordAsync(string password, CancellationToken cancellationToken = default);
        public void ForgotPassword();
    }
}