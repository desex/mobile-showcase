using System.Threading;
using System.Threading.Tasks;
using UnityEditor.AddressableAssets;

namespace Sotbifay.Builder.Editor
{
    public sealed class ClearCacheAddressablesStep : BaseBuildStep<BuildReport>
    {
        protected override Task<bool> ExecuteInternalAsync(BuildReport context, CancellationToken cancellationToken) => 
            Task.FromResult(ExecuteInternal(context));
        
        private static bool ExecuteInternal(BuildReport context)
        {
            var addressableAssetSettings = AddressableAssetSettingsDefaultObject.Settings;
            if (addressableAssetSettings == null)
                return true;

            var activePlayerDataBuilder = addressableAssetSettings.ActivePlayerDataBuilder;
            activePlayerDataBuilder.ClearCachedData();
            return true;
        }
    }
}