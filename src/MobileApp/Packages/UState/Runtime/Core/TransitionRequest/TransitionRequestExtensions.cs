using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UState
{
    public static class TransitionRequestExtensions
    {
        public static async UniTask WaitEnterAsync(this ITransitionRequest request, CancellationToken cancellationToken = default)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (request.InState)
                return;
            
            if(request.IsCancelled)
                throw new OperationCanceledException("Transition request is cancelled");

            var completionSource = new UniTaskCompletionSource();
            try
            {
                request.Entered += OnEntered;
                request.Canceled += OnCanceled;
                
                await completionSource.Task.AttachExternalCancellation(cancellationToken);
            }
            finally
            {
                request.Entered -= OnEntered;
                request.Canceled -= OnCanceled;
            }

            return;
            void OnEntered() => completionSource.TrySetResult();
            void OnCanceled() => completionSource.TrySetCanceled();
        }

        public static async UniTask WaitExecuteAsync(this ITransitionRequest request, CancellationToken cancellationToken = default)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));
            
            if(request.IsCancelled)
                throw new OperationCanceledException("Transition request is cancelled");
            
            var completionSource = new UniTaskCompletionSource();
            try
            {
                request.Executed += OnExecuted;
                request.Canceled += OnCanceled;
                
                await completionSource.Task.AttachExternalCancellation(cancellationToken);
            }
            finally
            {
                request.Executed -= OnExecuted;
                request.Canceled -= OnCanceled;
            }

            return;
            void OnExecuted() => completionSource.TrySetResult();
            void OnCanceled() => completionSource.TrySetCanceled();
        }
        
        public static async UniTask WaitExitAsync(this ITransitionRequest request, CancellationToken cancellationToken = default)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (request.InState == false)
                return;
            
            if(request.IsCancelled)
                throw new OperationCanceledException("Transition request is cancelled");

            var completionSource = new UniTaskCompletionSource();
            try
            {
                request.Exited += OnExited;
                request.Canceled += OnCanceled;
                
                await completionSource.Task.AttachExternalCancellation(cancellationToken);
            }
            finally
            {
                request.Exited -= OnExited;
                request.Canceled -= OnCanceled;
            }

            return;
            void OnExited() => completionSource.TrySetResult();
            void OnCanceled() => completionSource.TrySetCanceled();
        }
    }
}