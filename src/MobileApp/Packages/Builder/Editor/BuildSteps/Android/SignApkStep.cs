using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Sotbifay.Builder.Editor
{
    public sealed class SignApkStep : BaseBuildStep<BuildReport>
    {
        private readonly AndroidSignOptions _options;

        public SignApkStep(AndroidSignOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        protected override Task<bool> ExecuteInternalAsync(BuildReport context, CancellationToken cancellationToken) => 
            Task.FromResult(ExecuteInternal(context));
        
        private bool ExecuteInternal(BuildReport context)
        {
            if (_options.SignApk == false)
                return true;
            
            var buildPlayerOptions = context.BuildOptions;
            var apkFilePath = buildPlayerOptions.locationPathName;
            if (File.Exists(apkFilePath) && Path.GetExtension(apkFilePath) == ".apk")
            {
                SignApk(apkFilePath);
                Debug.Log($"The file '{apkFilePath}' has been signed.");
            }
            else
            {
                Debug.Log("The build is not an apk file");
            }

            return true;
        }

        private static void SignApk(string apkFilePath)
        {
            if (File.Exists(apkFilePath) == false)
                throw new FileNotFoundException("File not found", apkFilePath);

            var javaProcess = new JavaProcess();
            var processResult = javaProcess.SignApk(apkFilePath, Consts.Pk8FileFullPath, Consts.PemFileFullPath);

            if (processResult.HasError)
                throw new InvalidOperationException($"Failed to sign apk. Error: {processResult.Error}");
        }
    }
}