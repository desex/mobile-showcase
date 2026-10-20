using System.IO;

namespace Sotbifay.Builder.Editor
{
    public sealed class Java8Process : Process
    {
        private static readonly string FileName = Path.Combine(Consts.Jdk8DirectoryFullPath, "bin/java.exe");
        
        public Java8Process() : base(FileName)
        {
        }
    }
}