using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sotbifay.Images
{
    public sealed class UrlImageEntry : BaseImageEntry
    {
        private readonly Uri _url;

        public UrlImageEntry(Uri url, IImagePool imagePool) : base(imagePool)
        {
            _url = url ?? throw new ArgumentNullException(nameof(url));
        }

        protected override UniTask<Texture2D> LoadTextureAsync
        (
            IImagePool imagePool,
            IProgress<float> progress,
            CancellationToken cancellationToken
        ) => imagePool.LoadAsync(_url, progress, cancellationToken);

        protected override void Release(IImagePool imagePool) =>
            imagePool.Release(_url);
    }
}