using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace Sotbify.MobileApp.Relic
{
    public static class RelicInstanceExtensions
    {
        public static bool IsLoaded(this IRelicInstance relicInstance)
        {
            if (relicInstance is null)
                throw new ArgumentNullException(nameof(relicInstance));
            
            return relicInstance.Texture != null && relicInstance.Texture != Texture2D.blackTexture;
        }

        public static UniTask WaitLoadAsync(this IRelicInstance relicInstance, CancellationToken cancellationToken = default)
        {
            if (relicInstance is null)
                throw new ArgumentNullException(nameof(relicInstance));
            
            return UniTask.WaitUntil(relicInstance, static relicInstance => relicInstance.IsLoaded(), cancellationToken: cancellationToken);
        }
        
        public static Observable<Texture> LoadTextureObservable(this IRelicInstance relicInstance)
        {
            if (relicInstance is null)
                throw new ArgumentNullException(nameof(relicInstance));

            return Observable.FromEvent(h => relicInstance.Loaded += h, h => relicInstance.Loaded -= h)
                .Select(relicInstance, static (_, relicInstance) => relicInstance.Texture);
        }
        
        public static ReadOnlyReactiveProperty<Texture> TextureAsReadOnlyReactiveProperty(this IRelicInstance relicInstance)
        {
            if (relicInstance is null)
                throw new ArgumentNullException(nameof(relicInstance));

            return relicInstance.LoadTextureObservable()
                .ToReadOnlyReactiveProperty(relicInstance.Texture);
        }
    }
}