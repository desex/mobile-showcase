using System.IO;

namespace Sotbifay.Builder.Editor
{
    public sealed class JavaProcess : Process
    {
        private const string JavaFileName = 
#if UNITY_EDITOR_OSX
            "bin/java";
#elif UNITY_EDITOR_WIN
            "bin/java.exe";
#endif

        private static readonly string FileName = Path.Combine(Consts.JdkRootPath, JavaFileName);
        
        public JavaProcess() : base(FileName)
        {
        }
    }
}