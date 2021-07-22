//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

string target = Argument("Target", "Validate");

//////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
//////////////////////////////////////////////////////////////////////

string artifacts = "./Artifacts";
string auditArtifacts = $"{artifacts}/Audit";
string buildArtifacts = $"{artifacts}/Release";
string testArtifacts = $"{artifacts}";

string projectName = "Loki Logging Provider";
string solution = "./LokiLoggingProvider.sln";
string configuration = "Release";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Audit")
    .Does(() =>
    {
        DeleteDirectorySettings deleteSettings = new DeleteDirectorySettings
        {
            Force = true,
            Recursive = true,
        };

        DeleteDirectories(GetDirectories(auditArtifacts), deleteSettings);

        DotNetCoreTool("CycloneDX", new DotNetCoreToolSettings
        {
            ArgumentCustomization = args => args
                .Append(solution)
                .Append($"--json")
                .Append($"--out {auditArtifacts}")
        });
    });

Task("Clean")
    .Does(() =>
    {
        DotNetCoreClean(solution, new DotNetCoreCleanSettings
        {
            Configuration = configuration
        });
    });

Task("Restore")
    .Does(() =>
    {
        DotNetCoreRestore(solution);
    });

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        DotNetCoreBuild(solution, new DotNetCoreBuildSettings
        {
            Configuration = configuration,
            NoRestore = true,
            ArgumentCustomization = args => args
                .Append("-p:ContinuousIntegrationBuild=true")
        });
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
    {
        DeleteDirectorySettings deleteSettings = new DeleteDirectorySettings
        {
            Force = true,
            Recursive = true,
        };

        DeleteDirectories(GetDirectories("./Tests/**/TestResults"), deleteSettings);

        DotNetCoreTestSettings testSettings = new DotNetCoreTestSettings
        {
            ArgumentCustomization = args => args
                .Append("--collect:\"XPlat Code Coverage\"")
                .Append("--logger:\"console;verbosity=minimal\"")
                .Append("--logger:\"trx;\"")
                .Append("--nologo"),
            Configuration = configuration,
            NoBuild = true
        };

        DotNetCoreTest(solution, testSettings);

        string coverageHistory = $"{testArtifacts}/CoverageHistory";
        string coverageReports = $"{testArtifacts}/CoverageReports";
        string reportTypes = "Html;Cobertura;";
        string classFilters = "-Gogoproto*;-Logproto*";

        DeleteDirectories(GetDirectories(coverageReports), deleteSettings);

        DotNetCoreTool("reportgenerator", new DotNetCoreToolSettings
        {
            ArgumentCustomization = args => args
                .Append($"-reports:\"./Tests/*.UnitTests/TestResults/*/coverage.cobertura.xml\"")
                .Append($"-reporttypes:\"{reportTypes}\"")
                .Append($"-targetdir:\"{coverageReports}/UnitTests\"")
                .Append($"-historydir:\"{coverageHistory}/UnitTests\"")
                .Append($"-title:\"{projectName} Unit Tests\"")
                .Append($"-verbosity:\"Error\"")
                .Append($"-classfilters:\"{classFilters}\"")
        });
    });

Task("Pack")
    .IsDependentOn("Test")
    .Does(() =>
    {
        DeleteDirectorySettings deleteSettings = new DeleteDirectorySettings
        {
            Force = true,
            Recursive = true,
        };

        DeleteDirectories(GetDirectories($"{buildArtifacts}"), deleteSettings);

        DotNetCorePackSettings settings = new DotNetCorePackSettings
        {
            Configuration = configuration,
            NoBuild = true,
            OutputDirectory = $"{buildArtifacts}"
        };

        DotNetCorePack("./Sources/LokiLoggingProvider", settings);
    });

Task("Validate")
    .IsDependentOn("Pack")
    .Does(() =>
    {
        FilePathCollection filePaths = GetFiles($"{buildArtifacts}/**/*.nupkg");

        foreach (FilePath filePath in filePaths)
        {
            DotNetCoreTool("validate", new DotNetCoreToolSettings
            {
                ArgumentCustomization = args => args
                    .Append("package")
                    .Append("local")
                    .Append(filePath.ToString())
            });
        }
    });

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
