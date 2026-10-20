using System;
using System.Collections.Generic;
using System.IO;

namespace Sotbifay.Builder.Editor
{
    public static class AdbProcessExtensions
    {
        public static ProcessResult UploadFile(this AdbProcess adbProcess, string filePath, string filePathOnDevice)
        {
            if (adbProcess is null)
                throw new ArgumentNullException(nameof(adbProcess));
            
            if(File.Exists(filePath) == false)
                throw new FileNotFoundException("File not found", filePath);

            if (string.IsNullOrEmpty(filePathOnDevice))
                throw new ArgumentException("Can not be null or empty", nameof(filePathOnDevice));

            filePathOnDevice = filePathOnDevice.Replace('\\', '/');
            var fileDirectoryOnDevice = (Path.GetDirectoryName(filePathOnDevice!) ?? string.Empty).Replace('\\', '/');

            var mkdirProcessResult = adbProcess.Execute($"shell mkdir -p \"{fileDirectoryOnDevice}\"");
            if (mkdirProcessResult.HasError)
                return mkdirProcessResult;

            var pushProcessResult = adbProcess.Execute($"push -p \"{filePath}\" \"{filePathOnDevice}\"");
            return mkdirProcessResult.Merge(pushProcessResult);
        }
        
        public static (IReadOnlyList<string> packageNames, ProcessResult processResult) GetInstalledPackages(this AdbProcess adbProcess)
        {
            if (adbProcess is null)
                throw new ArgumentNullException(nameof(adbProcess));
            
            var packagesListResult = adbProcess.Execute("shell pm list packages");
            if (packagesListResult.HasError)
                return (Array.Empty<string>(), packagesListResult);
            
            var packageNames = packagesListResult.Output
                .Replace("package:", string.Empty)
                .Replace("Package:", string.Empty)
                .Split(Environment.NewLine);
                
            return (packageNames, packagesListResult);
        }

        public static ProcessResult InstallApk(this AdbProcess adbProcess, string apkFilePath)
        {
            if (adbProcess is null)
                throw new ArgumentNullException(nameof(adbProcess));
            
            if(File.Exists(apkFilePath) == false)
                throw new FileNotFoundException("File not found", apkFilePath);

            return adbProcess.Execute($"install -r -g -d \"{apkFilePath}\"");
        }
        
        public static ProcessResult UninstallPackage(this AdbProcess adbProcess, string packageName)
        {
            if (adbProcess is null)
                throw new ArgumentNullException(nameof(adbProcess));
            
            if(string.IsNullOrEmpty(packageName))
                throw new ArgumentException("Package name cannot be null or empty", nameof(packageName));
            
            return adbProcess.Execute($"uninstall {packageName}");
        }
        
        public static ProcessResult SetAutoStart(this AdbProcess adbProcess, string packageName)
        {
            if (adbProcess is null)
                throw new ArgumentNullException(nameof(adbProcess));
            
            if(string.IsNullOrEmpty(packageName))
                throw new ArgumentException("Package name cannot be null or empty", nameof(packageName));
            
            var processResult1 = adbProcess.Execute($"shell setprop persist.pxr.force.home {packageName},{Consts.UnityPlayerActivity}");
            var processResult2 = adbProcess.Execute($"shell setprop persist.pvr.default.home {packageName}");

            return processResult1.Merge(processResult2);
        }

        public static ProcessResult LaunchApplication(this AdbProcess adbProcess, string packageName)
        {
            if (adbProcess is null)
                throw new ArgumentNullException(nameof(adbProcess));
            
            if(string.IsNullOrEmpty(packageName))
                throw new ArgumentException("Package name cannot be null or empty", nameof(packageName));
            
            return adbProcess.Execute($"shell am start -n {packageName}/{Consts.UnityPlayerActivity}");
        }

        public static ProcessResult Reboot(this AdbProcess adbProcess)
        {
            if (adbProcess is null)
                throw new ArgumentNullException(nameof(adbProcess));
            
            return adbProcess.Execute("reboot");
        }
    }
}