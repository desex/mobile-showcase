using System;
using System.Threading;
using System.Threading.Tasks;
using Sotbifay.Builder.Editor;
using Sotbify.MobileApp.Builder.Editor.Options;
using UnityEditor;

namespace Sotbify.MobileApp.Builder.Editor.Steps
{
    public sealed class ApplyIOSOptionsStep : BaseBuildStep<BuildReport>
    {
        private readonly IOSOptions _options;

        public ApplyIOSOptionsStep(IOSOptions iosOptions)
        {
            _options = iosOptions ?? throw new ArgumentNullException(nameof(iosOptions));
        }

        protected override Task<bool> ExecuteInternalAsync(BuildReport context, CancellationToken cancellationToken)
        {
            PlayerSettings.iOS.appleDeveloperTeamID = _options.IOSTeamID;
            PlayerSettings.applicationIdentifier = _options.IOSPackageName;
            PlayerSettings.productName = _options.PartnerName == "MockPartnerA" ? "Mock App A"
                : _options.PartnerName == "MockPartnerB" ? "Mock App B"
                : "Development " + _options.PartnerName;
            
            return Task.FromResult(true);
        }
    }
}