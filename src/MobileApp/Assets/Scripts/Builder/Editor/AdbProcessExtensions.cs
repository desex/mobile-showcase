using System;
using System.IO;
using Sotbifay.Builder.Editor;

namespace Sotbify.MobileApp.Builder.Editor
{
    public static class AdbProcessExtensions
    {
        public static ProcessResult InstallApk(this AdbProcess adbProcess, string apkFilePath)
        {
            if (adbProcess is null)
                throw new ArgumentNullException(nameof(adbProcess));
            
            if(File.Exists(apkFilePath) == false)
                throw new FileNotFoundException("File not found", apkFilePath);

            return adbProcess.Execute($"install -r \"{apkFilePath}\"");
        }
        
        public static ProcessResult LaunchApplication(this AdbProcess adbProcess, string packageName)
        {
            if (adbProcess is null)
                throw new ArgumentNullException(nameof(adbProcess));
            
            if(string.IsNullOrEmpty(packageName))
                throw new ArgumentException("Package name cannot be null or empty", nameof(packageName));
            
            return adbProcess.Execute($"shell monkey -p {packageName} -c android.intent.category.LAUNCHER 1");
        }
    }
}