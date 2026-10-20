using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor.PackageManager;

namespace Sotbifay.Builder.Editor
{
    public static class PackageUtils
    {
        public static string GetPackageNameByCallerFile([CallerFilePath]string filePath = null)
        {
            if (filePath is null)
                throw new ArgumentNullException(nameof(filePath));
            
            var fullFilePath = Path.GetFullPath(filePath);
            
            foreach (var package in PackageInfo.GetAllRegisteredPackages())
            {
                var packageResolvedPath = package.resolvedPath;
                if (fullFilePath.Contains(packageResolvedPath))
                    return package.name;
            }
            
            return string.Empty;
        }

        public static string GetPackageFullPathByName(string packageName)
        {
            if (packageName is null)
                throw new ArgumentNullException(nameof(packageName));

            return Path.GetFullPath("Packages/" + packageName);
        }
    }
}