using System;
using System.Linq;
using System.Threading;
using Nuke.Common;
using Nuke.Common.CI.GitLab;
using Nuke.Common.Tools.Git;
using Serilog;

public partial class Build
{
    [Parameter("Should increment version")] 
    readonly bool ShouldIncrement = true;
    
    Target IncrementVersion => target => target
        .After(CleanArtifacts)
        .After(RestoreNuGetPackages)
        .Executes(() =>
        {
            if (ShouldIncrement == false)
                return;
            
            var version = IncrementVersionAsync();
            Log.Information($"Version bumped to {version}");
        });
    
    Target IncrementVersionAndPush => target => target
        .After(CleanArtifacts)
        .After(RestoreNuGetPackages)
        .OnlyWhenStatic(() => GitLab.Instance != null)
        .OnlyWhenStatic(() => string.Equals(GitLab.Instance.PipelineSource, "push", StringComparison.OrdinalIgnoreCase))
        .Executes(() =>
        {
            var version = IncrementVersionAsync();
            var tag = $"v{version}";
            
            GitTasks.Git("config user.name CI");
            GitTasks.Git("config user.email ci@example.invalid");

            GitTasks.Git("add -A");
            try
            {
                GitTasks.Git($"commit -m {"bump version"}");
            }
            catch
            {
                Log.Information("Nothing to commit");
                return;
            }
            
            var branch = Environment.GetEnvironmentVariable("CI_COMMIT_REF_NAME");

            if (string.IsNullOrWhiteSpace(branch))
                throw new Exception("CI_COMMIT_REF_NAME is empty");

            GitTasks.Git($"push origin HEAD:{branch} -o ci.skip");

            if (TagExists(tag) == false)
            {
                var message = $"Release {tag}";
                GitTasks.Git($"tag -a {tag} -m {message}");
                GitTasks.Git($"push origin {tag}");
            }
            else
            {
                Log.Warning("Tag {tag} already exists. Skipping tag creation.", tag);
            }
        });

    private static bool TagExists(string tag) => GitTasks.Git($"tag -l {tag}")
        .Any(x => string.IsNullOrWhiteSpace(x.Text) == false);
    
    private static string GetCurrentTagOrEmpty()
    {
        try
        {
            return GitTasks.Git("describe --tags --abbrev=0")
                .Single()
                .Text;
        }
        catch (InvalidOperationException)
        {
            return string.Empty;
        }
    }

    private static string GetChangelogBetweenTags(string currentTag)
    {
        string range;
        try
        {
            var previousTag = GitTasks.Git($"describe --tags --abbrev=0 {currentTag}^")
                .Single()
                .Text;

            range = $"{previousTag}..{currentTag}";
        }
        catch (InvalidOperationException)
        {
            range = currentTag;
        }

        var commits = GitTasks.Git($"log {range} --no-merges --pretty=format:{"- %B%n"}");
        var commitTexts = commits.Select(x => x.Text)
            .Where(x => string.IsNullOrWhiteSpace(x) == false);
        
        return string.Join(Environment.NewLine, commitTexts);
    }
}