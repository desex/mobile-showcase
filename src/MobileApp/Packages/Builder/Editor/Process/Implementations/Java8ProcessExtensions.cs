using System;
using System.IO;
using System.Text;

namespace Sotbifay.Builder.Editor
{
    public static class Java8ProcessExtensions
    {
        public static ProcessResult SignApk
        (
            this Java8Process javaProcess,
            string apkFilePath,
            string signedApkFilePath,
            string apkSignerFilePath,
            string pk8FilePath,
            string x509PemFilePath
        )
        {
            if(javaProcess is null)
                throw new ArgumentNullException(nameof(javaProcess));
            
            if(File.Exists(apkFilePath) == false)
                throw new FileNotFoundException("File not found", apkFilePath);
            
            if (string.IsNullOrEmpty(signedApkFilePath))
                throw new ArgumentException("Parameter can not be null or empty", nameof(signedApkFilePath));
            
            if(File.Exists(apkSignerFilePath) == false)
                throw new FileNotFoundException("File not found", apkSignerFilePath);
            
            if(File.Exists(pk8FilePath) == false)
                throw new FileNotFoundException("File not found", pk8FilePath);
            
            if(File.Exists(x509PemFilePath) == false)
                throw new FileNotFoundException("File not found", x509PemFilePath);

            var argumentsStringBuilder = new StringBuilder();
            argumentsStringBuilder
                .Append("-jar")
                .Append(' ').Append('"').Append(apkSignerFilePath).Append('"')
                .Append(' ').Append('"').Append(x509PemFilePath).Append('"')
                .Append(' ').Append('"').Append(pk8FilePath).Append('"')
                .Append(' ').Append('"').Append(apkFilePath).Append('"')
                .Append(' ').Append('"').Append(signedApkFilePath).Append('"');
            
            return javaProcess.Execute(argumentsStringBuilder.ToString());
        }
    }
}