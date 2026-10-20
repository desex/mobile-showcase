using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace Sotbify.MobileApp.RelicsDescriptionWindow
{
    public interface IRelicDescription
    {
        public ReadOnlyReactiveProperty<RelicDescriptionModel> RelicDescriptionProperty { get; }
        public ReadOnlyReactiveProperty<Texture> TextureProperty { get; }
        public Observable<Unit> CloseObservable { get; }

        public UniTask LoadDescription(int relicId, CancellationToken cancellationToken = default);
        public void Close();
    }
}