using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;

namespace Sotbifay.Launcher.Common.Tools.Editor
{
    [InitializeOnLoad]
    public static class YamlMergeRegistrar
    {
        private const string YamlMergeFileName =
#if UNITY_EDITOR_OSX
            "UnityYAMLMerge";
#elif UNITY_EDITOR_WIN
            "UnityYAMLMerge.exe";
#endif
        
        static YamlMergeRegistrar()
        {
            if (string.IsNullOrEmpty(StartGitProcess("config --local --get merge.unityyamlmerge.driver")) == false)
                return;
            
            var yamlMergeToolPath = GetYamlMergeToolPath();
            if(string.IsNullOrEmpty(yamlMergeToolPath))
                return;
            
            StartGitProcess("config merge.unityyamlmerge.name \"Unity SmartMerge\"");
            
            var driverValue = $"'{yamlMergeToolPath}' merge -p \"$BASE\" \"$REMOTE\" \"$LOCAL\" \"$MERGED\"";
            StartGitProcess($"config merge.unityyamlmerge.driver \"{driverValue}\"");
        }

        private static string StartGitProcess(string arguments)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            
            using var process = Process.Start(processStartInfo);
            if (process is null)
                throw new InvalidOperationException("Process could not be started");
         
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            
            process.WaitForExit();
            
            return string.IsNullOrEmpty(error) == false 
                ? throw new InvalidOperationException(error)
                : output;
        }

        private static string GetYamlMergeToolPath()
        {
            var path = Path.Combine(GetToolsDirectoryPath(), YamlMergeFileName);

#if UNITY_EDITOR_OSX
                path = path.Replace('\\', '/');
#endif
            return path;
        }

        private static string GetToolsDirectoryPath()
        {
#if UNITY_EDITOR_OSX
            var directoryName = Path.GetDirectoryName(EditorApplication.applicationPath) ?? string.Empty;
            return Path.GetFullPath(Path.Combine(EditorApplication.applicationPath, "Contents", "Tools")).Replace('\\', '/');
#elif UNITY_EDITOR_WIN
            var directoryName = Path.GetDirectoryName(EditorApplication.applicationPath) ?? string.Empty;
            return Path.GetFullPath(Path.Combine(directoryName, "..", "Data", "Tools"));
#endif
        }
    }
}