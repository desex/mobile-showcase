using System.Threading;
using System.Threading.Tasks;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace Sotbifay.Builder.Editor
{
    public sealed class BuildAddressablesStep : BaseBuildStep<BuildReport>
    {
        protected override Task<bool> ExecuteInternalAsync(BuildReport context, CancellationToken cancellationToken) => 
            Task.FromResult(ExecuteInternal(context));
        
        private static bool ExecuteInternal(BuildReport context)
        {
            var addressableAssetSettings = AddressableAssetSettingsDefaultObject.Settings;
            if (addressableAssetSettings == null)
                return true;

            AddressableAssetSettings.BuildPlayerContent(out var result);
            return string.IsNullOrEmpty(result.Error);
        }
    }
}