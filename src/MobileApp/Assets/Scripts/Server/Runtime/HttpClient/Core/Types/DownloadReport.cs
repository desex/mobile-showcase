using System;
using UnityEngine;

namespace Sotbify.MobileApp.Server.HttpClient.Core.Types
{
    public readonly struct DownloadReport
    {
        public long BytesDownloaded { get; }
        public long TotalBytes { get; }
        public double BytesPerSecond { get; }
        
        public float NormalizedProgress => TotalBytes > 0 ? Mathf.Clamp01((float)((double)BytesDownloaded / TotalBytes)) : 0f;

        public TimeSpan EstimatedRemaining
        {
            get
            {
                if (TotalBytes <= 0 || BytesPerSecond <= 0)
                    return TimeSpan.Zero;
                
                var remainingBytes = TotalBytes - BytesDownloaded;
                if (remainingBytes <= 0)
                    return TimeSpan.Zero;
                
                var remainingSeconds = remainingBytes / BytesPerSecond;
                return remainingSeconds > 0 ? TimeSpan.FromSeconds(remainingSeconds) : TimeSpan.Zero;
            }
        }

        public DownloadReport(long bytesDownloaded, long totalBytes, double bytesPerSecond)
        {
            BytesDownloaded = bytesDownloaded;
            TotalBytes = totalBytes;
            BytesPerSecond = bytesPerSecond;
        }

        public override string ToString() => $"({BytesDownloaded}/{TotalBytes} B | {(NormalizedProgress * 100f):F2}% | ETA {EstimatedRemaining:hh\\:mm\\:ss})";
    }
}