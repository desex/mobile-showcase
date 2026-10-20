using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Sotbifay.Builder.Editor;
using UnityEditor;
using UnityEditor.Build;

namespace Sotbify.MobileApp.Builder.Editor.Options
{
    public sealed class ApplyCommonIOSOptionsStep : BaseBuildStep<BuildReport>
    {
        private readonly CommonOptions _options;

        public ApplyCommonIOSOptionsStep(CommonOptions commonOptions)
        {
            _options = commonOptions ?? throw new ArgumentNullException(nameof(commonOptions));
        }

        protected override Task<bool> ExecuteInternalAsync(BuildReport context, CancellationToken cancellationToken)
        {
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            var targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            var targetName = NamedBuildTarget.FromBuildTargetGroup(targetGroup);

            context.BuildId = _options.BuildId;
            context.BuildType = _options.BuildType;

            var scriptingImplementation = PlayerSettings.GetScriptingBackend(targetName);
            if (scriptingImplementation is ScriptingImplementation.IL2CPP)
            {
                var compilerConfiguration = _options.BuildType is BuildType.Debug
                    ? Il2CppCompilerConfiguration.Debug
                    : Il2CppCompilerConfiguration.Release;

                PlayerSettings.SetIl2CppCompilerConfiguration(targetName, compilerConfiguration);
            }

            var buildPlayerOptions = context.BuildOptions;
            buildPlayerOptions.options |= _options.BuildType switch
            {
                BuildType.Debug => BuildOptions.Development,
                _ => BuildOptions.None
            };

            buildPlayerOptions.options |= _options.UseProfiler
                ? BuildOptions.ConnectWithProfiler | BuildOptions.AllowDebugging | BuildOptions.WaitForPlayerConnection
                : BuildOptions.None;

            buildPlayerOptions.options |= _options.UseProfiler && _options.DeepProfiler
                ? BuildOptions.EnableDeepProfilingSupport
                : BuildOptions.None;

            if (Directory.Exists(_options.OutputDirectory) == false)
                Directory.CreateDirectory(_options.OutputDirectory);

            if (buildPlayerOptions.target is BuildTarget.iOS)
            {
                var directoryName =  $"{PlayerSettings.productName}_{PlayerSettings.bundleVersion}_{_options.BuildType}_{_options.BuildId}";
                
                buildPlayerOptions.locationPathName = Path.Combine(_options.OutputDirectory, directoryName);
            }

            context.BuildOptions = buildPlayerOptions;

            return Task.FromResult(true);
        }
    }
}