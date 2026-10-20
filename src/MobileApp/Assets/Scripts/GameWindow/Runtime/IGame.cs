using System.Threading;
using Cysharp.Threading.Tasks;
using ObservableCollections;

namespace Sotbify.MobileApp.PlayWindow
{
    public interface IGame
    {
        public IReadOnlyObservableList<IPartnerModel> Partners { get; }
        public UniTask LoadPartners(CancellationToken cancellationToken = default);
    }
}