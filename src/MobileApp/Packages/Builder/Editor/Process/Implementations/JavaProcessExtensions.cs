using System;
using System.IO;
using System.Text;

namespace Sotbifay.Builder.Editor
{
    public static class JavaProcessExtensions
    {
        private const string ApkSignerFile = "apksigner.jar";

        public static ProcessResult SignApk(this JavaProcess javaProcess, string apkFilePath, string pk8FilePath, string x509PemFilePath)
        {
            if(javaProcess is null)
                throw new ArgumentNullException(nameof(javaProcess));
            
            if(File.Exists(apkFilePath) == false)
                throw new FileNotFoundException("File not found", apkFilePath);
            
            if(File.Exists(pk8FilePath) == false)
                throw new FileNotFoundException("File not found", pk8FilePath);
            
            if(File.Exists(x509PemFilePath) == false)
                throw new FileNotFoundException("File not found", x509PemFilePath);
         
            var apkSignerFilePath = GetApkSignerFilePath();
            var argumentsStringBuilder = new StringBuilder();
            argumentsStringBuilder
                .Append("-jar ").Append('"').Append(apkSignerFilePath).Append('"')
                .Append(" sign")
                .Append(" --key ").Append('"').Append(pk8FilePath).Append('"')
                .Append(" --cert ").Append('"').Append(x509PemFilePath).Append('"')
                .Append(' ').Append('"').Append(apkFilePath).Append('"');

            return javaProcess.Execute(argumentsStringBuilder.ToString());
        }

        public static ProcessResult CheckSignApk(this JavaProcess javaProcess, string apkFilePath)
        {
            if(javaProcess is null)
                throw new ArgumentNullException(nameof(javaProcess));
            
            if(File.Exists(apkFilePath) == false)
                throw new FileNotFoundException("File not found", apkFilePath);

            var apkSignerFilePath = GetApkSignerFilePath();
            var argumentsStringBuilder = new StringBuilder();
            argumentsStringBuilder
                .Append("-jar ").Append('"').Append(apkSignerFilePath).Append('"')
                .Append(' ')
                .Append("verify --print-certs")
                .Append(' ')
                .Append('"').Append(apkFilePath).Append('"');

            return javaProcess.Execute(argumentsStringBuilder.ToString());
        }

        private static string GetApkSignerFilePath()
        {
            var files = Directory.GetFiles(Consts.SdkRootPath, ApkSignerFile, SearchOption.AllDirectories);
            var apkSignerFilePath = files.Length > 0 ? files[0] : string.Empty;
            
            if (string.IsNullOrEmpty(apkSignerFilePath))
                throw new FileNotFoundException($"'{ApkSignerFile}' not found at '{Consts.SdkRootPath}'");

            return apkSignerFilePath;
        }
    }
}