using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Sotbifay.Builder.Editor
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CommonOptions
    {
        [Option("buildId")]
        public string BuildId { get; set; } = Guid.NewGuid().ToString();
        
        [Option("type")]
        public BuildType BuildType { get; set; } = BuildType.Release;
        
        [Option("debugSymbolsIncluded")]
        public bool DebugSymbolsIncluded { get; set; } = false;

        [Option("useProfiler")]
        public bool UseProfiler { get; set; } = false;
        
        [Option("deepProfiler")]
        public bool DeepProfiler { get; set; } = false;

        [Option("outputDirectory")] 
        public string OutputDirectory { get; set; } = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Build"));
        
        [Option("defines")]
        public List<string> Defines { get; set; } = new();

        [Option("removeDefines")]
        public List<string> RemoveDefines { get; set; } = new();
        
        [Option("buildPrefix")]
        public string BuildPrefix { get; set; } = string.Empty;
        
        [Option("buildSuffix")]
        public string BuildSuffix { get; set; } = string.Empty;
    }
}