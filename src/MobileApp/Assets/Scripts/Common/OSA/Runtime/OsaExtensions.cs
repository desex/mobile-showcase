using System;
using System.Threading;
using Com.ForbiddenByte.OSA.Core;
using Cysharp.Threading.Tasks;
using R3;

namespace Sotbify.MobileApp.Common.OSA
{
    public static class OsaExtensions
    {
        public static UniTask WaitInitializeAsync(this IOSA osa, CancellationToken cancellationToken = default)
        {
            return osa is not null 
                ? UniTask.WaitUntil(osa, static osa => osa.IsInitialized, cancellationToken: cancellationToken)
                : throw new ArgumentNullException(nameof(osa));
        } 
        
        public static Observable<Unit> InitializeObservable(this IOSA osa)
        {
            return osa != null
                ? Observable.FromEvent(h => osa.Initialized += h, h => osa.Initialized -= h) 
                : throw new ArgumentNullException(nameof(osa));
        }
        
        public static ReadOnlyReactiveProperty<bool> InitializedReactiveProperty(this IOSA osa)
        {
            if(osa == null)
                throw new ArgumentNullException(nameof(osa));

            return osa.InitializeObservable()
                .Select(static _ => true)
                .ToReadOnlyReactiveProperty(osa.IsInitialized);
        }
    }
}