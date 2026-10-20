using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.Android.Types;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Build;

namespace Sotbifay.Builder.Editor
{
    public sealed class ApplyCommonOptionsStep : BaseBuildStep<BuildReport>
    {
        private readonly CommonOptions _options;

        public ApplyCommonOptionsStep(CommonOptions commonOptions)
        {
            _options = commonOptions ?? throw new ArgumentNullException(nameof(commonOptions));
        }
        
        protected override Task<bool> ExecuteInternalAsync(BuildReport context, CancellationToken cancellationToken) => 
            Task.FromResult(ExecuteInternal(context));

        private bool ExecuteInternal(BuildReport context)
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

            if (buildPlayerOptions.target is BuildTarget.Android)
            {
                var fileNameStringBuilder = new StringBuilder();
                
                UserBuildSettings.DebugSymbols.level = _options.DebugSymbolsIncluded
                    ? _options.BuildType is BuildType.Debug
                        ? DebugSymbolLevel.Full
                        : DebugSymbolLevel.SymbolTable
                    : DebugSymbolLevel.None;

                if (string.IsNullOrEmpty(_options.BuildPrefix) == false)
                {
                    fileNameStringBuilder.Append(_options.BuildPrefix);
                    fileNameStringBuilder.Append('_');
                }
                
                fileNameStringBuilder
                    .Append(PlayerSettings.productName).Append('_')
                    .Append(PlayerSettings.bundleVersion).Append('_')
                    .Append(_options.BuildType).Append('_')
                    .Append(_options.BuildId);

                if (string.IsNullOrEmpty(_options.BuildSuffix) == false)
                {
                    fileNameStringBuilder.Append('_');
                    fileNameStringBuilder.Append(_options.BuildSuffix);
                }

                fileNameStringBuilder.Append(".apk");
                
                buildPlayerOptions.locationPathName = Path.Combine(_options.OutputDirectory, fileNameStringBuilder.ToString());
            }
            
            context.BuildOptions = buildPlayerOptions;
            
            DefineUtils.AddDefines(_options.Defines);
            DefineUtils.RemoveDefines(_options.RemoveDefines);

            return true;
        }
    }
}