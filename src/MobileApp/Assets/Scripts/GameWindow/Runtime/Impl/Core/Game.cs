using System.Threading;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using Sotbify.MobileApp.PlayWindow.Adapter;

namespace Sotbify.MobileApp.PlayWindow.Core
{
    public class Game : IGame
    {
        private readonly ObservableList<IPartnerModel> _partners = new ();
        public IReadOnlyObservableList<IPartnerModel> Partners => _partners;
        
        public async UniTask LoadPartners(CancellationToken cancellationToken = default)
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
        
        private IPartnerModel CreatePartnerModel()
        {
            // Здесь была реализация, не могу показать более
            throw new global::System.NotImplementedException();
        }
    }
}