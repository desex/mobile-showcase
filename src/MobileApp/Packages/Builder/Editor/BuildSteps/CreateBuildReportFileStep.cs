using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Sotbifay.Builder.Editor
{
    public sealed class CreateBuildReportFileStep : BaseBuildStep<BuildReport>
    {
        private static readonly JsonSerializerSettings Settings = new()
        {
            Converters = new List<JsonConverter>
            {
                new StringEnumConverter()
            },
        };
        
        private readonly CommonOptions _options;

        public CreateBuildReportFileStep(CommonOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        protected override Task<bool> ExecuteInternalAsync(BuildReport context, CancellationToken cancellationToken) => 
            Task.FromResult(ExecuteInternal(context));
        
        private bool ExecuteInternal(BuildReport context)
        {
            var fileName = $"build_report_{context.BuildId}.json";
            var buildReportFilePath = Path.Combine(_options.OutputDirectory, fileName);
            ToJsonFile(context, buildReportFilePath);
            return true;
        }
        
        private static void ToJsonFile(BuildReport buildReport, string filePath)
        {
            if (buildReport is null)
                throw new ArgumentNullException(nameof(buildReport));

            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("Argument can not be null or empty", nameof(filePath));

            var json = JsonConvert.SerializeObject(buildReport, Formatting.Indented, Settings);
            File.WriteAllText(filePath, json);
        }
    }
}