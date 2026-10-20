using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;

namespace Sotbifay.Builder.Editor
{
    public sealed class ApplyAndroidOptionsStep : BaseBuildStep<BuildReport>
    {
        private readonly AndroidOptions _options;

        public ApplyAndroidOptionsStep(AndroidOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        protected override Task<bool> ExecuteInternalAsync(BuildReport context, CancellationToken cancellationToken) => 
            Task.FromResult(ExecuteInternal(context));
        
        private bool ExecuteInternal(BuildReport context)
        {
            PlayerSettings.Android.splitApplicationBinary = _options.SplitApplicationBinary;
            return true;
        }
    }
}