using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sotbifay.Images;
using UnityEngine;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.Relic
{
    public sealed class ImageRelicInstance : BaseRelicInstance
    {
        private readonly Uri _imageUrl;
        private readonly IImagePool _imagePool;

        public ImageRelicInstance
        (
            Uri imageUrl,
            Color backgroundColor,
            IImagePool imagePool,
            ILogger logger
        ) : base(backgroundColor, logger)
        {
            _imageUrl = imageUrl ?? throw new ArgumentNullException(nameof(imageUrl));
            _imagePool = imagePool ?? throw new ArgumentNullException(nameof(imagePool));
        }

        protected override async UniTask<Texture> LoadInternalAsync(CancellationToken cancellationToken)
        {
            var texture = await _imagePool.LoadAsync(_imageUrl, cancellationToken: CancellationToken.None);
            cancellationToken.ThrowIfCancellationRequested();

            return texture;
        }
    }
}