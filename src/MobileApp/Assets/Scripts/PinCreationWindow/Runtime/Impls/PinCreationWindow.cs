using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbifay.MobileApp.Logger;
using Sotbify.MobileApp.PinPassword;

namespace Sotbify.MobileApp.PinCreationWindow
{
    public class PinCreationWindow : IPinCreationWindow
    {
        private readonly Subject<bool> _action = new();
        private readonly IPinPasswordService _pinPasswordService;
        private readonly ILogger _logger;

        public PinCreationWindow(IPinPasswordService pinPasswordService, ILogger logger)
        {
            _pinPasswordService = pinPasswordService ?? throw new ArgumentNullException(nameof(pinPasswordService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool IsChangingPin { get; private set; }
        public Observable<bool> ActionObservable => _action;

        public void SetChangingMode(bool isChangingPin) {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void Close() {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public UniTask SavePasswordAsync(string password, CancellationToken cancellationToken)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}
