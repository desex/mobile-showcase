using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using VContainer.Unity;
using ILogger = Sotbifay.MobileApp.Logger.ILogger;

namespace Sotbify.MobileApp.UI.Common.Texture
{
    public sealed class IntervalTextureConverter : ITextureConverter, IInitializable, IDisposable
    {
        private const double TextureCreateIntervalInSeconds = 0.5d;

        private readonly Queue<CreateTextureContract> _createContracts = new();
        private readonly ILogger _logger;
        
        private IDisposable _disposable;

        public IntervalTextureConverter(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initialize()
        {
            _disposable = Observable.Interval(TimeSpan.FromSeconds(TextureCreateIntervalInSeconds))
                .Subscribe(this, (_, converter) => converter.ConvertSingle());
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }

        public UniTask<Texture2D> CreateTextureFromBytesAsync(byte[] bytes,
            CancellationToken cancellationToken = default)
        {
            var contract = new CreateTextureContract(bytes, cancellationToken);
            _createContracts.Enqueue(contract);
            return WaitForTextureCreateAsync(contract, cancellationToken);
        }

        public UniTask<Texture2D> CreateTextureFromMemoryAsync(ReadOnlyMemory<byte> memory,
            CancellationToken cancellationToken = default)
        {
            var contract = new CreateTextureContract(memory, cancellationToken);
            _createContracts.Enqueue(contract);
            return WaitForTextureCreateAsync(contract, cancellationToken);
        }

        public async UniTask<Texture2D> CreateTextureFromFileAsync(string filePath,
            CancellationToken cancellationToken = default)
        {
            if (File.Exists(filePath) == false)
            {
                throw new FileNotFoundException(filePath);
            }

            var bytes = await File.ReadAllBytesAsync(filePath, cancellationToken).AsUniTask();
            cancellationToken.ThrowIfCancellationRequested();
            return await CreateTextureFromBytesAsync(bytes, cancellationToken);
        }

        public UniTask SaveTextureToFileAsync(Texture2D texture, string savePath,
            CancellationToken cancellationToken = default)
        {
            if (texture == null)
            {
                throw new NullReferenceException(nameof(texture));
            }

            var bytes = texture.EncodeToPNG();
            return File.WriteAllBytesAsync(savePath, bytes, cancellationToken).AsUniTask();
        }

        private async UniTask<Texture2D> WaitForTextureCreateAsync(CreateTextureContract contract,
            CancellationToken cancellationToken)
        {
            await UniTask.WaitUntil(() => contract.IsCompleted, cancellationToken: cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            return contract.Texture;
        }

        private void ConvertSingle()
        {
            while (_createContracts.TryDequeue(out var contract))
            {
                if (contract.IsAborted)
                {
                    continue;
                }

                contract.Create();
                return;
            }
        }
        
        private class CreateTextureContract
        {
            private readonly ReadOnlyMemory<byte> _memory;
            private readonly byte[] _bytes;
            private readonly CancellationToken _cancellationToken;

            public bool IsCompleted { get; private set; }
            public Texture2D Texture { get; private set; }

            public bool IsAborted => _cancellationToken.IsCancellationRequested;

            public CreateTextureContract(byte[] bytes, CancellationToken cancellationToken)
            {
                _bytes = bytes;
                _cancellationToken = cancellationToken;
            }

            public CreateTextureContract(ReadOnlyMemory<byte> memory, CancellationToken cancellationToken)
            {
                _memory = memory;
                _cancellationToken = cancellationToken;
            }

            public void Create()
            {
                Texture = _bytes == null
                    ? _memory.CreateTextureFromBytes(false)
                    : _bytes.CreateTextureFromBytes(false);

                IsCompleted = true;
            }
        }
    }
}