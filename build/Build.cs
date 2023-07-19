using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Serilog;
using static Nuke.Common.Tools.DotNet.DotNetTasks;


[GitHubActions(
    "publish",
    GitHubActionsImage.UbuntuLatest,
    OnPushBranches = new[] { "publish" },
    InvokedTargets = new[] { nameof(Publish) },
    ImportSecrets = new[] { nameof(NuGetApiKey) })]
[GitHubActions(
    "ci",
    GitHubActionsImage.UbuntuLatest,
    OnPullRequestBranches = new []{ "develop", "main" }, 
    OnPushBranches = new[] { "main" },
    InvokedTargets = new[] { nameof(Test), nameof(Pack) })]
class Build : NukeBuild
{
    [GitRepository] readonly GitRepository GitRepository;


    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            // dotnet restore (without clean)
            // dotnet clean restore
        });

    // nuke --solution1 
    // parameters.json
    // env var: SOLUTION1 / NUKE_SOLUTION1
    [Parameter] readonly string Solution1;

    // this is auto injected from ./config/parameters.json
    [Solution] readonly Solution Solution;

    Target Restore => _ => _
        .Executes(() =>
        {
            // dotnet restore <solution>
            DotNet($"restore {Solution.Path}");
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            // dotnet build <solution> --configuration Debug/Release
            DotNetBuild(_ => _
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                // .EnableNoRestore()
                .SetNoRestore(FinishedTargets.Contains(Restore))
            );
        });


    // v- root directory
    // <parent>/artifacts/tests-results (unix)
    // <parent>\artifacts\tests-results

    AbsolutePath TestResultsDirectory => ArtifactsDirectory / "tests-results";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath PackageDirectory => ArtifactsDirectory / "package";

    UnixRelativePath ResultsDirectoryRelativePathFromRootDirectory
        => RootDirectory.GetUnixRelativePathTo(TestResultsDirectory);

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            // RootDirectory: where the .nuke folder is 

            // dotnet test sln --no-build --results-directory <path>
            DotNetTest(_ => _
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetResultsDirectory(TestResultsDirectory)
                // ScheduledTargets: meant to be run
                .SetNoBuild(ScheduledTargets.Contains(Compile))
            );
        });


    [GitVersion] readonly GitVersion GitVersion;

    [Parameter] readonly string Version;

    Target Pack => _ => _
        .DependsOn(Compile)
        .Requires(() => Version)
        .Executes(() =>
        {
            Log.Information("GitVersion = {Version}", GitVersion.MajorMinorPatch);

            DotNetPack(_ => _
                .SetProject(Solution)
                .SetOutputDirectory(PackageDirectory)
                .SetConfiguration(Configuration)
                .SetNoBuild(FinishedTargets.Contains(Compile))
                .SetVersion(Version)
                .SetProcessExitHandler(p => p.ExitCode switch
                {
                    0 or 1 => null,
                    2 => p.AssertZeroExitCode(),
                    _ => throw new Exception($"{p.ExitCode} tests have failed"),
                })
                .DisableProcessAssertZeroExitCode()
            );
        });

    // nuke ::secrets
    [Parameter] [Secret] readonly string NuGetApiKey;
    [Parameter] readonly string NuGetSource;

    Target Publish => _ => _
        .DependsOn(Clean)
        .DependsOn(Test)
        .DependsOn(Pack)
        .Requires(() => NuGetApiKey)
        .Requires(() => NuGetSource)
        .Executes(() =>
        {
            DotNetNuGetPush(_ => _
                .SetTargetPath(PackageDirectory.GetFiles().Last())
                .SetApiKey(NuGetApiKey)
            );
        });
}
