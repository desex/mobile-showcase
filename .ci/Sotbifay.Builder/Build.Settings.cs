using System;
using System.IO;
using System.Linq;
using Nuke.Common;
using Nuke.Common.IO;
using Types;

public partial class Build
{
    [Parameter("Partner name"), Required]
    readonly string PartnerName = string.Empty;

    private static AbsolutePath SettingFilesDirectory => RootDirectory / "BuildSettings";
    private static AbsolutePath RiobetSettingFilesDirectory => SettingFilesDirectory / "Riobet";
    private static AbsolutePath CryptobossSettingFilesDirectory => SettingFilesDirectory / "Cryptoboss";
    private static AbsolutePath CryptobossDevSettingFilesDirectory => SettingFilesDirectory / "CryptobossDev";
    private static AbsolutePath RiobetDevSettingFilesDirectory => SettingFilesDirectory / "RiobetDev";
    private static AbsolutePath UnitySettingFilesDirectory => ProjectDirectory / "Assets" / "StreamingAssets";
    private static AbsolutePath BackupSettingFilesDirectory => TemporaryDirectory / "Nuke_Backup";


    Target ChangeSettingFiles => target => target
        .After(CleanArtifacts)
        .After(IncrementVersion)
        .Executes(() =>
        {
            if (SettingFilesDirectory.Exists() == false)
                throw new NullReferenceException("There is no build settings folder");

            if (UnitySettingFilesDirectory.Exists() == false)
                throw new NullReferenceException("There is no streaming asset folder");

            var sourceDirectory = PartnerName == "MockPartnerA" ? RiobetSettingFilesDirectory
                : PartnerName == "MockPartnerB" ? CryptobossSettingFilesDirectory
                : PartnerName == "MockPartnerBDev" ? CryptobossDevSettingFilesDirectory
                : PartnerName == "MockPartnerADev" ? RiobetDevSettingFilesDirectory
                : throw new ArgumentException($"There is no such partner as {PartnerName}");

            if (Directory.Exists(sourceDirectory) == false)
                throw new DirectoryNotFoundException($"Source settings directory was not found: {sourceDirectory}");

            var sourceFiles = Directory.GetFiles(sourceDirectory)
                .Select(Path.GetFileName)
                .ToList();

            if (sourceFiles.Count == 0)
                return;

            Directory.CreateDirectory(BackupSettingFilesDirectory);

            var filesToBackup = sourceFiles
                .Where(file => File.Exists(Path.Combine(UnitySettingFilesDirectory, file)))
                .ToList();

            foreach (var file in filesToBackup)
            {
                var originalFile = Path.Combine(UnitySettingFilesDirectory, file);

                if (File.Exists(originalFile))
                    File.Copy(originalFile, Path.Combine(BackupSettingFilesDirectory, file), true);
            }

            foreach (var file in sourceFiles)
            {
                var swapFile = Path.Combine(sourceDirectory, file);
                File.Copy(swapFile, Path.Combine(UnitySettingFilesDirectory, file), true);
            }
        });

    Target RestoreSettingFiles => target => target
        .After(CleanArtifacts)
        .After(BuildIOSUnity)
        .AssuredAfterFailure()
        .Executes(() =>
        {
            if (Directory.Exists(BackupSettingFilesDirectory) == false)
                return;

            var filesToRestore = Directory.GetFiles(BackupSettingFilesDirectory)
                .Select(Path.GetFileName)
                .ToList();
            
            foreach (var fileName in filesToRestore)
            {
                var backupFile = Path.Combine(BackupSettingFilesDirectory, fileName);
                if (File.Exists(backupFile))
                {
                    File.Copy(backupFile, Path.Combine(UnitySettingFilesDirectory, fileName), true);
                }
            }

            Directory.Delete(BackupSettingFilesDirectory, true);
        });
}