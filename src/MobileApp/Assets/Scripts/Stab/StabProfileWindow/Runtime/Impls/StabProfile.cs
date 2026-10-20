using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.Services.Browser;
using Sotbify.MobileApp.Utils;

namespace Stab.StabProfileWindow
{
    public class StabProfile : IStabProfile, IDisposable
    {
        private readonly Subject<EProfileCategory> _profileCategory = new ();
        private readonly Subject<Unit> _deleteAccount = new ();
        private readonly IBrowser _browser;

        public Observable<EProfileCategory> ProfileCategoryObservable => _profileCategory;
        public Observable<Unit> DeleteAccountObservable => _deleteAccount;

        public StabProfile(IBrowser browser)
        {
            _browser = browser;
        }

        public void OpenSideWindow(EProfileCategory profileCategory)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public async UniTask OpenWebPage(string url, CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void DeleteAccount()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }

        public void Dispose()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}