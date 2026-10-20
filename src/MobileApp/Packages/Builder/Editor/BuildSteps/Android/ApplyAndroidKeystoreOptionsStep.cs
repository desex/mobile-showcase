using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Sotbifay.Builder.Editor
{
    public sealed class ApplyAndroidKeystoreOptionsStep : BaseBuildStep<BuildReport>
    {
        private readonly AndroidKeystoreOptions _options;

        public ApplyAndroidKeystoreOptionsStep(AndroidKeystoreOptions commonOptions)
        {
            _options = commonOptions ?? throw new ArgumentNullException(nameof(commonOptions));
        }
        
        protected override Task<bool> ExecuteInternalAsync(BuildReport context, CancellationToken cancellationToken) => 
            Task.FromResult(ExecuteInternal(context));
        
        private bool ExecuteInternal(BuildReport context)
        {
            if (string.IsNullOrEmpty(_options.KeyStoreName) || File.Exists(_options.KeyStoreName) == false)
            {
                PlayerSettings.Android.useCustomKeystore = false;
                Debug.LogWarning("Keystore is not specified. Keystore usage is disabled");
                return true;
            }

            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystoreName = _options.KeyStoreName;
            PlayerSettings.Android.keystorePass = _options.KeyStorePassword;
            PlayerSettings.Android.keyaliasName = _options.AliasName;
            PlayerSettings.Android.keyaliasPass = _options.AliasPassword;
                
            return true;
        }
    }
}