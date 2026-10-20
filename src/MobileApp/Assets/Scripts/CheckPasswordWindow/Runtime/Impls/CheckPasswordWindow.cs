using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.PinPassword;

namespace Sotbify.MobileApp.CheckPasswordWindow
{
    public class CheckPasswordWindow : ICheckPasswordWindow
    {
        private readonly Subject<bool> _isPasswordValid = new();
        private readonly Subject<Unit> _forgotPassword = new();
        private readonly IPinPasswordService _pinPasswordService;

        public bool IsChangingPin { get; private set; }

        public CheckPasswordWindow(IPinPasswordService pinPasswordService)
        {
            _pinPasswordService = pinPasswordService; 
        }
        
        public Observable<bool> IsPasswordValid => _isPasswordValid;
        public Observable<Unit> ForgotPasswordObservable => _forgotPassword;

        public void SetChangingMode(bool isChangingPin) 
            {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public UniTask CheckPasswordAsync(string password, CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void ForgotPassword() 
            {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}