using System;
using Nuke.Common.IO;

namespace Extensions;

public static class AbsolutePathExtensions
{
    public static void SafeRemove(this AbsolutePath path)
    {
        ArgumentNullException.ThrowIfNull(path);
        
        if (path.FileExists())
        {
            path.DeleteFile();
        }
        else if(path.DirectoryExists())
        {
            path.DeleteDirectory();
        }
    }
    
    public static AbsolutePath WriteFromBase64(this AbsolutePath filePath, string base64String)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        ArgumentNullException.ThrowIfNull(base64String);

        var fileBytes = Convert.FromBase64String(base64String);
        return filePath.WriteAllBytes(fileBytes);
    }
}