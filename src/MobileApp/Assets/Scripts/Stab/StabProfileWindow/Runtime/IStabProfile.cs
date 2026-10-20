using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Sotbify.MobileApp.Utils;

namespace Stab.StabProfileWindow
{
    public interface IStabProfile
    {
        public Observable<EProfileCategory> ProfileCategoryObservable { get; }
        public Observable<Unit> DeleteAccountObservable { get; }
        
        public void OpenSideWindow(EProfileCategory profileCategory);
        public UniTask OpenWebPage(string url, CancellationToken cancellationToken = default);
        public void DeleteAccount();
    }
}