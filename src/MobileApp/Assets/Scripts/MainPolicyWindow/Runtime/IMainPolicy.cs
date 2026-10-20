using System.Threading;
using Cysharp.Threading.Tasks;
using R3;

namespace MainPolicyWindow
{
    public interface IMainPolicy
    {
        public Observable<Unit> CloseButtonPressed { get; }

        public UniTask OpenUrl(string url, CancellationToken cancellationToken = default);
        public void Close();
    }
}