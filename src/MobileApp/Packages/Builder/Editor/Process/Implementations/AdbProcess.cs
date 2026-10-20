using System.IO;

namespace Sotbifay.Builder.Editor
{
    public sealed class AdbProcess : Process
    {
        private const string ADBFileName = 
#if UNITY_EDITOR_OSX
            "platform-tools/adb";
#elif UNITY_EDITOR_WIN
            "platform-tools/adb.exe";
#endif

        private static readonly string FileName = Path.Combine(Consts.SdkRootPath, ADBFileName);
        
        public AdbProcess() : base(FileName)
        {
        }
    }
}