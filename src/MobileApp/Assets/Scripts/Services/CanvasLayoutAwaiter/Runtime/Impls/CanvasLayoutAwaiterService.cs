using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sotbify.MobileApp.Services.CanvasLayoutAwaiter
{
    public class CanvasLayoutAwaiterService : ICanvasLayoutAwaiterService
    {
        public UniTask WaitForNextLayoutAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var awaiter = new NextCanvasLayoutAwaiter(cancellationToken);
            return awaiter.Task;
        }

        private sealed class NextCanvasLayoutAwaiter : IDisposable
        {
            private readonly UniTaskCompletionSource _completionSource = new();
            private readonly CancellationTokenRegistration _cancellationRegistration;

            public NextCanvasLayoutAwaiter(CancellationToken cancellationToken)
            {
                Canvas.willRenderCanvases += OnWillRenderCanvases;
                _cancellationRegistration = cancellationToken.Register(OnCanceled);
            }

            public UniTask Task => _completionSource.Task;

            private void OnWillRenderCanvases()
            {
                Dispose();
                _completionSource.TrySetResult();
            }

            private void OnCanceled()
            {
                Dispose();
                _completionSource.TrySetCanceled();
            }

            public void Dispose()
            {
                Canvas.willRenderCanvases -= OnWillRenderCanvases;
                _cancellationRegistration.Dispose();
            }
        }
    }
}