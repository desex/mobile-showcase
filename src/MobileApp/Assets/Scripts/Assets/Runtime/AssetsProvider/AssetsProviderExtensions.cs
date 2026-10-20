using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Sotbifay.MobileApp.Assets
{
    public static class AssetsProviderExtensions
    {
        public static UniTask<bool> IsDownloadRequiredAsync
        (
            this IAssetsProvider assetsProvider,
            object key,
            CancellationToken cancellationToken = default
        )
        {
            if(assetsProvider is null)
                throw new ArgumentNullException(nameof(assetsProvider));

            return assetsProvider.GetDownloadSizeAsync(key, cancellationToken)
                .ContinueWith(size => size > 0);
        }
    }
}