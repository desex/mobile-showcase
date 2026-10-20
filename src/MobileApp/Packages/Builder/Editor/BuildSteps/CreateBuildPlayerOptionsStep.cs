using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;

namespace Sotbifay.Builder.Editor
{
    public sealed class CreateBuildPlayerOptionsStep : BaseBuildStep<BuildReport>
    {
        protected override Task<bool> ExecuteInternalAsync(BuildReport context, CancellationToken cancellationToken) => 
            Task.FromResult(ExecuteInternal(context));
        
        private static bool ExecuteInternal(BuildReport context)
        {
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            
            var targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            var scenes = EditorBuildSettings.scenes
                .Select(scene => scene.path)
                .ToArray();
            
            context.BuildOptions = new BuildPlayerOptions
            {
                target = buildTarget,
                targetGroup = targetGroup,
                scenes = scenes
            };
            
            return true;
        }
    }
}