using System;
using System.IO;
using System.Linq;
using Sotbifay.Builder.Editor;
using UnityEditor;

namespace Sotbify.MobileApp.Builder.Editor
{
    public static class InstallMenuItem
    {
        [MenuItem(Consts.MenuItemRoot + "Install APK")]
        public static void InstallApk()
        {
            var apkFilePath = GetApkFilePath();
            var packageName = GetPackageName(apkFilePath);

            if (CheckPackageIsInstalled(packageName))
            {
                var removeDialogResult = EditorUtility.DisplayDialog
                (
                    $"Removing package {packageName}",
                    $"Package \"{packageName}\" is already installed. Do you want to remove it?",
                    "Yes",
                    "No"
                );

                if (removeDialogResult)
                    RemovePackage(packageName);
            }

            InstallApk(apkFilePath);

            var obbFilePath = GetObbFilePath(apkFilePath);
            if (string.IsNullOrEmpty(obbFilePath) == false)
            {
                var versionCode = GetVersionCode(apkFilePath);
                UploadObbFile(obbFilePath, packageName, versionCode);
            }

            // SetAutoStart(packageName);
            LaunchApp(packageName);
        }

        [MenuItem(Consts.MenuItemRoot + "Uninstall APK from device")]
        public static void DeleteInstalledApk()
        {
            var apkFilePath = GetApkFilePath();
            var packageName = GetPackageName(apkFilePath);

            if (CheckPackageIsInstalled(packageName))
            {
                var removeDialogResult = EditorUtility.DisplayDialog
                (
                    $"Removing package {packageName}",
                    $"Package \"{packageName}\" is already installed. Do you want to remove it?",
                    "Yes",
                    "No"
                );

                if (removeDialogResult)
                    RemovePackage(packageName);
            }
        }

        private static string GetSaveApkFilePath()
        {
            var directory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var directoryPath = EditorUtility.SaveFolderPanel
            (
                "Select build directory",
                directory,
                ""
            );

            if (Directory.Exists(directoryPath) == false)
                throw new DirectoryNotFoundException(directoryPath);

            var fileName = $"{PlayerSettings.productName}_{PlayerSettings.bundleVersion}.apk";
            return Path.Combine(directoryPath, fileName);
        }

        private static string GetApkFilePath()
        {
            var apkFilePath = EditorUtility.OpenFilePanel("Select apk file", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "apk");
            if (File.Exists(apkFilePath) == false)
                throw new FileNotFoundException(apkFilePath);
            
            return apkFilePath;
        }

        private static string GetPackageName(string apkFilePath)
        {
            var aaptProcess = new AaptProcess();
            var (packageName, processResult) = aaptProcess.GetPackageName(apkFilePath);
            if (processResult.HasError)
                throw new InvalidOperationException($"Failed to retrieve the package name. Error: {processResult.Error}");

            return packageName;
        }

        private static bool CheckPackageIsInstalled(string packageName)
        {
            var adbProcess = new AdbProcess();
            var (packageNames, processResult) = adbProcess.GetInstalledPackages();
            if (processResult.HasError)
                throw new InvalidOperationException($"Failed to get installed packages. Error: {processResult.Error}");

            return packageNames.Any(pName => pName.Equals(packageName, StringComparison.OrdinalIgnoreCase));
        }

        private static void RemovePackage(string packageName)
        {
            var adbProcess = new AdbProcess();
            var processResult = adbProcess.UninstallPackage(packageName);
            if (processResult.HasError)
                throw new InvalidOperationException($"Failed to remove package. Error: {processResult.Error}");
        }

        private static void InstallApk(string apkFilePath)
        {
            var adbProcess = new AdbProcess();
            var processResult = adbProcess.InstallApk(apkFilePath);
            if (processResult.HasError)
                throw new InvalidOperationException($"Failed to install apk. Error: {processResult.Error}");
        }

        private static string GetObbFilePath(string apkFilePath = null)
        {
            var directoryPath = string.IsNullOrEmpty(apkFilePath)
                ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                : Path.GetDirectoryName(apkFilePath);

            var obbFilePath = EditorUtility.OpenFilePanel("Select obb file", directoryPath, "obb");
            if (string.IsNullOrEmpty(obbFilePath))
                return string.Empty;

            if (File.Exists(obbFilePath) == false)
                throw new FileNotFoundException(obbFilePath);

            return obbFilePath;
        }

        private static int GetVersionCode(string apkFilePath)
        {
            var aaptProcess = new AaptProcess();
            var (versionCode, processResult) = aaptProcess.GetPackageVersionCode(apkFilePath);
            if (processResult.HasError)
                throw new InvalidOperationException($"Failed to retrieve the package version code. Error: {processResult.Error}");

            return versionCode;
        }

        private static void UploadObbFile(string obbFilePath, string packageName, int versionCode)
        {
            var adbProcess = new AdbProcess();
            var processResult = adbProcess.UploadFile(obbFilePath, $"/sdcard/Android/obb/{packageName}/main.{versionCode}.{packageName}.obb");
            if (processResult.HasError)
                throw new InvalidOperationException($"Failed to upload obb. Error: {processResult.Error}");
        }

        private static void SetAutoStart(string packageName)
        {
            var adbProcess = new AdbProcess();
            var processResult = adbProcess.SetAutoStart(packageName);
            if (processResult.HasError)
                throw new InvalidOperationException($"Failed to set auto start. Error: {processResult.Error}");
        }

        private static void LaunchApp(string packageName)
        {
            var adbProcess = new AdbProcess();
            var processResult = adbProcess.LaunchApplication(packageName);
            if (processResult.HasError)
                throw new InvalidOperationException($"Failed to launch application. Error: {processResult.Error}");
        }
    }
}