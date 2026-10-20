using System;
using UnityEditor;
using UnityEngine;

namespace Sotbifay.Builder.Editor
{
    [Serializable]
    public class BuildReport
    {
        public string BuildId { get; set; }
        public BuildType BuildType { get; set; }

        public string ApplicationName { get; set; } = Application.productName;
        public string ApplicationVersion { get; set; } = Application.version;

        public long StartBuildTimestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        public long FinishBuildTimestamp { get; set; }
        
        public BuildPlayerOptions BuildOptions { get; set; }
    }
}