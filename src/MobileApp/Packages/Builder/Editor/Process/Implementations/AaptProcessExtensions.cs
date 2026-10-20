using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Sotbifay.Builder.Editor
{
    public static class AaptProcessExtensions
    {
        private static readonly Regex PackageNameRegex = new("^package: name='(?<packageName>\\S+)'");
        private static readonly Regex VersionCodeRegex = new("versionCode='(?<versionCode>\\d+)'");
        
        public static (string packageName, ProcessResult processResult) GetPackageName(this AaptProcess aaptProcess, string apkFilePath)
        {
            if (aaptProcess is null)
                throw new ArgumentNullException(nameof(aaptProcess));
            
            if(File.Exists(apkFilePath) == false)
                throw new FileNotFoundException("File not found", apkFilePath);
            
            var processResult = aaptProcess.Execute($"dump badging \"{apkFilePath}\"");
            if (processResult.HasError)
                return (string.Empty, processResult);

            var match = PackageNameRegex.Match(processResult.Output);
            var packageNameGroup = match.Groups["packageName"];
            return packageNameGroup.Success
                ? (packageNameGroup.Value, processResult) 
                : (string.Empty, processResult);
        }
        
        public static (int versionCode, ProcessResult processResult) GetPackageVersionCode(this AaptProcess aaptProcess, string apkFilePath)
        {
            if (aaptProcess is null)
                throw new ArgumentNullException(nameof(aaptProcess));
            
            if(File.Exists(apkFilePath) == false)
                throw new FileNotFoundException("File not found", apkFilePath);
            
            var processResult = aaptProcess.Execute($"dump badging \"{apkFilePath}\"");
            if (processResult.HasError)
                return (0, processResult);

            var match = VersionCodeRegex.Match(processResult.Output);
            var versionCodeGroup = match.Groups["versionCode"];
            return versionCodeGroup.Success && int.TryParse(versionCodeGroup.Value, out var versionCode)
                ? (versionCode, processResult) 
                : (0, processResult);
        }
    }
}