using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sotbify.MobileApp.TextureProvider
{
    public static class TextureProviderExtensions
    {
        public static UniTask<Sprite> LoadSpriteAsync
        (
            this ITextureProvider textureProvider,
            Uri url,
            IProgress<float> progress = null,
            CancellationToken cancellationToken = default
        )
        {
            if (textureProvider is null)
                throw new ArgumentNullException(nameof(textureProvider));

            if (url is null)
                throw new ArgumentNullException(nameof(url));

            return textureProvider.LoadTextureAsync(url, progress, cancellationToken)
                .ContinueWith(texture => Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f));
        }
    }
}