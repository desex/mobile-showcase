using System;
using System.Diagnostics;
using System.Text;
using Debug = UnityEngine.Debug;

namespace Sotbifay.Builder.Editor
{
    public class Process
    {
        private readonly string _fileName;

        public Process(string fileName)
        {
            _fileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        }
        
        public ProcessResult Execute(string arguments)
        {
            var processStartInfo = GetProcessStartInfo(arguments);

            var processStartInfoString = ProcessStartInfoToString(processStartInfo);
            Debug.Log($"[Process]: {processStartInfoString}");

            using var process = System.Diagnostics.Process.Start(processStartInfo);
            if (process is null)
                throw new InvalidOperationException("Process could not be started");
         
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            
            process.WaitForExit();
            
            return new ProcessResult(output, error);
        }
        
        private ProcessStartInfo GetProcessStartInfo(string arguments)
        {
            return new ProcessStartInfo
            {
                FileName = _fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
        }

        private static string ProcessStartInfoToString(ProcessStartInfo processStartInfo)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append("File name: ")
                .AppendLine(processStartInfo.FileName)
                .Append("Arguments: ")
                .Append(processStartInfo.Arguments);
            
            return stringBuilder.ToString();
        }
    }
}