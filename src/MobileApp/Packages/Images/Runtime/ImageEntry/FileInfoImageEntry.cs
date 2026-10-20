using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sotbifay.Images
{
    public sealed class FileInfoImageEntry : BaseImageEntry
    {
        private readonly FileInfo _file;

        public FileInfoImageEntry(FileInfo file, IImagePool imagePool) : base(imagePool)
        {
            _file = file ?? throw new ArgumentNullException(nameof(file));
        }

        protected override UniTask<Texture2D> LoadTextureAsync
        (
            IImagePool imagePool,
            IProgress<float> progress,
            CancellationToken cancellationToken
        ) => imagePool.LoadAsync(_file, progress, cancellationToken);

        protected override void Release(IImagePool imagePool) => 
            imagePool.Release(_file);
    }
}