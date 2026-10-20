using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Sotbifay.Builder.Editor
{
    public sealed class BuildStep : BaseBuildStep<BuildReport>
    {
        protected override Task<bool> ExecuteInternalAsync(BuildReport context, CancellationToken cancellationToken) => 
            Task.FromResult(ExecuteInternal(context));
        
        private static bool ExecuteInternal(BuildReport context)
        {
            var logger = Debug.unityLogger;
            var parentHandler = logger.logHandler;
            var buildLogHandler = new BuildLogHandler(parentHandler);
            logger.logHandler = buildLogHandler;

            var buildReport = BuildPipeline.BuildPlayer(context.BuildOptions);
            logger.logHandler = parentHandler;

            var summary = buildReport.summary;
            if (summary.result is BuildResult.Succeeded)
                return true;
            
            var errorStringBuilder = new StringBuilder();
            errorStringBuilder.Append("Failed to build. Total errors: ");
            errorStringBuilder.Append(summary.totalErrors);
            errorStringBuilder.AppendLine();
            errorStringBuilder.AppendJoin(Environment.NewLine, buildLogHandler.Errors);

            throw new InvalidOperationException(errorStringBuilder.ToString());
        }
    }
}