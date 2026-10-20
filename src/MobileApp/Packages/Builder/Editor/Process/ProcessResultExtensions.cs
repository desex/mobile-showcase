using System;

namespace Sotbifay.Builder.Editor
{
    public static class ProcessResultExtensions
    {
        public static ProcessResult Merge(this ProcessResult firstProcessResult, ProcessResult secondProcessResult)
        {
            var output = $"{firstProcessResult.Output}{Environment.NewLine}{secondProcessResult.Output}";
            var error = $"{firstProcessResult.Error}{Environment.NewLine}{secondProcessResult.Error}";
            return new ProcessResult(output, error);
        }
    }
}