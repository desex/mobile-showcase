using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sotbifay.Builder.Editor
{
    public sealed class SetFinishBuildTimestampStep : BaseBuildStep<BuildReport>
    {
        protected override Task<bool> ExecuteInternalAsync(BuildReport context, CancellationToken cancellationToken) => 
            Task.FromResult(ExecuteInternal(context));
        
        private static bool ExecuteInternal(BuildReport context)
        {
            context.FinishBuildTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return true;
        }
    }
}