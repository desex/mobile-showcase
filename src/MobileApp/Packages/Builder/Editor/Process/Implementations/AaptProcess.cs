using System.IO;

namespace Sotbifay.Builder.Editor
{
    public sealed class AaptProcess : Process
    {
        private const string AaptFile =
#if UNITY_EDITOR_OSX
                "aapt";
#elif UNITY_EDITOR_WIN
            "aapt.exe";
#endif
          
        private static readonly string FileName = GetAaptFilePath();
        
        public AaptProcess() : base(FileName)
        {
        }

        private static string GetAaptFilePath()
        {
            var files = Directory.GetFiles(Consts.SdkRootPath, AaptFile, SearchOption.AllDirectories);
            var apkSignerFilePath = files.Length > 0 ? files[0] : string.Empty;
            
            if (string.IsNullOrEmpty(apkSignerFilePath))
                throw new FileNotFoundException($"'{AaptFile}' not found at '{Consts.SdkRootPath}'");

            return apkSignerFilePath;
        }
    }
}