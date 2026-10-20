using System.IO;
using Extensions;
using Nuke.Common.IO;
using Nuke.Common.Tools.Unity;
using Serilog;

public partial class Build
{
    private const string VersionBumper = "Sotbify.MobileApp.Builder.Editor.VersionBumper.Bump";
    private static readonly AbsolutePath SystemTemp = Path.GetTempPath();
    
    private string IncrementVersionAsync()
    {
        Log.Information("Opening Unity for version bump...");
        
        UnityTasks.Unity(x => x
            .SetProjectPath(ProjectDirectory)
            .SetExecuteMethod(VersionBumper)
            .SetBatchMode(true)
            .SetQuit(true)
            .SetNoGraphics(true)
        );
        
        Log.Information("Unity project settings are updated");
        
        var resultFile = SystemTemp / "nuke_result.txt";
        
        var newVersion = "undefined";
        
        if (resultFile.FileExists())
        {
            newVersion = resultFile.ReadAllText().Trim();
            resultFile.SafeRemove();
        }
        else
            Log.Warning($"Version file could not be found");

        return newVersion;
    }
}