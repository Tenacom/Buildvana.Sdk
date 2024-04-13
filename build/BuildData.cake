// Copyright (C) Tenacom and contributors. Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

#nullable enable

using NuGet.Versioning;

// ---------------------------------------------------------------------------------------------
// CIPlatform: Continuous Integration platform under which the script runs
// ---------------------------------------------------------------------------------------------

enum CIPlatform
{
    None,
    GitHub,
    GitLab,
    Unsupported,
}

// ---------------------------------------------------------------------------------------------
// BuildData: a record to hold build configuration data
// ---------------------------------------------------------------------------------------------

/*
 * Summary : Holds configuration data for the build.
 */
sealed class BuildData
{
    /*
    * Summary : Initializes a new instance of the BuildData class.
    * Params  : context - The Cake context.
    */
    public BuildData(ICakeContext context)
    {
        var ciPlatform = context.EnvironmentVariable<bool>("GITHUB_ACTIONS", false) ? CIPlatform.GitHub
            : context.EnvironmentVariable<bool>("GITLAB_CI", false) ? CIPlatform.GitLab
            : context.EnvironmentVariable<bool>("CI", false)
                || context.EnvironmentVariable<bool>("CONTINUOUS_INTEGRATION", false)
                || context.EnvironmentVariable<bool>("TF_BUILD", false)
                || context.EnvironmentVariable<bool>("TRAVIS", false)
                || context.EnvironmentVariable<bool>("APPVEYOR", false)
                || context.EnvironmentVariable<bool>("CIRCLECI", false)
                || context.HasEnvironmentVariable("TEAMCITY_VERSION")
                || context.HasEnvironmentVariable("JENKINS_URL")
                ? CIPlatform.Unsupported : CIPlatform.None;

        context.Ensure(ciPlatform is CIPlatform.None or CIPlatform.GitHub or CIPlatform.GitLab, 255, "Running under an unsupported CI");
        context.Ensure(context.TryGetRepositoryInfo(out var repository), 255, "Cannot determine repository owner and name.");
        var solutionPath = context.GetFiles("*.sln").FirstOrDefault() ?? context.Fail<FilePath>(255, "Cannot find a solution file.");
        var solution = context.ParseSolution(solutionPath);
        var configuration = context.Argument("configuration", "Release");
        var artifactsPath = new DirectoryPath("artifacts").Combine(configuration);
        var testResultsPath = new DirectoryPath("TestResults");
        var (versionStr, @ref, isPublicRelease, isPrerelease) = context.GetVersionInformation();
        var version = SemanticVersion.Parse(versionStr);
        var branch = context.GetCurrentGitBranch();
        var msBuildSettings = new DotNetMSBuildSettings {
            MaxCpuCount = 1,
            ContinuousIntegrationBuild = ciPlatform is not CIPlatform.None,
            NoLogo = true,
        };

        (LatestVersion, LatestStableVersion) = context.GitGetLatestVersions();

        CIPlatform = ciPlatform;
        RepositoryHostUrl = repository.HostUrl;
        RepositoryOwner = repository.Owner;
        RepositoryName = repository.Name;
        Remote = repository.Remote;
        Ref = @ref;
        Branch = branch;
        ArtifactsPath = artifactsPath;
        TestResultsPath = testResultsPath;
        SolutionPath = solutionPath;
        Solution = solution;
        Configuration = configuration;
        VersionStr = versionStr;
        Version = version;
        IsPublicRelease = isPublicRelease;
        IsPrerelease = isPrerelease;
        MSBuildSettings = msBuildSettings;

        context.Information("Build configuration data:");
        context.Information($"CI platform           : {CIPlatform}");
        context.Information($"Repository            : {RepositoryHostUrl}/{RepositoryOwner}/{RepositoryName}");
        context.Information($"Git remote name       : {Remote}");
        context.Information($"Git reference         : {Ref}");
        context.Information($"Branch                : {Branch}");
        context.Information($"Solution              : {SolutionPath.GetFilename()}");
        context.Information($"Version               : {Version}");
        context.Information($"Public release        : {(IsPublicRelease ? "yes" : "no")}");
        context.Information($"Prerelease            : {(IsPrerelease ? "yes" : "no")}");
        context.Information($"Latest version        : {LatestVersion?.ToString() ?? "(none)"}");
        context.Information($"Latest stable version : {LatestStableVersion?.ToString() ?? "(none)"}");
    }

    /*
     * Summary : Gets a value indicating under which CI platform (if any) the script runs
     */
    public CIPlatform CIPlatform { get; }

    /*
     * Summary : Gets the repository host URL (e.g. "https://github.com" for a repository hosted on GitHub.)
     */
    public string RepositoryHostUrl { get; }

    /*
     * Summary : Gets the repository owner (e.g. "Tenacom" for repository Tenacom/SomeLibrary.)
     */
    public string RepositoryOwner { get; }

    /*
     * Summary : Gets the repository owner (e.g. "SomeLibrary" for repository Tenacom/SomeLibrary.)
     */
    public string RepositoryName { get; }

    /*
     * Summary : Gets the name of the Git remote that points to the main repository
     *           (usually "origin" in cloud builds, "upstream" when working locally on a fork.)
     */
    public string Remote { get; }

    /*
     * Summary : Gets Git's HEAD reference or SHA.
     */
    public string Ref { get; private set; }

    /*
     * Summary : Gets Git's HEAD branch name, or the empty string if not on a branch.
     */
    public string Branch { get; }

    /*
     * Summary : Gets the path of the directory where build artifacts are stored.
     */
    public DirectoryPath ArtifactsPath { get; }

    /*
     * Summary : Gets the path of the directory where test results and coverage reports are stored.
     */
    public DirectoryPath TestResultsPath { get; }

    /*
     * Summary : Gets the path of the solution file.
     */
    public FilePath SolutionPath { get; }

    /*
     * Summary : Gets the parsed solution.
     */
    public SolutionParserResult Solution { get; }

    /*
     * Summary : Gets the configuration to build.
     */
    public string Configuration { get; }

    /*
     * Summary : Gets the version to build, as a string computed by Nerdbank.GitVersioning.
     */
    public string VersionStr { get; private set; }

    /*
     * Summary : Gets the version to build, as a SemanticVersion object.
     */
    public SemanticVersion Version { get; private set; }

    /*
     * Summary : Gets the latest version published, if any, as a SemanticVersion object.
     */
    public SemanticVersion? LatestVersion { get; private set; }

    /*
     * Summary : Gets the latest stable version published, if any, as a SemanticVersion object.
     */
    public SemanticVersion? LatestStableVersion { get; private set; }

    /*
     * Summary : Gets a value that indicates whether a public release can be built.
     * Value   : True if Git's HEAD is on a public release branch, as indicated in version.json;
     *           otherwise, false.
     */
    public bool IsPublicRelease { get; private set; }

    /*
     * Summary : Gets a value that indicates whether the version to build is a prerelease.
     */
    public bool IsPrerelease { get; private set; }

    /*
     * Summary : Gets the MSBuild settings to use for DotNet aliases.
     */
    public DotNetMSBuildSettings MSBuildSettings { get; }

    /*
     * Summary : Update build configuration data, typically after a commit.
     * Params  : context - The Cake context.
     */
    public void Update(ICakeContext context)
    {
        (VersionStr, Ref, IsPublicRelease, IsPrerelease) = context.GetVersionInformation();
        Version = SemanticVersion.Parse(VersionStr);
        context.Information("Updated build configuration data:");
        context.Information($"Git reference  : {Ref}");
        context.Information($"Version        : {Version}");
        context.Information($"Public release : {(IsPublicRelease ? "yes" : "no")}");
        context.Information($"Prerelease     : {(IsPrerelease ? "yes" : "no")}");
    }
}
