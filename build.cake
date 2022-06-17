//////////////////////////////////////////////////////////////////////
// ENVIRONMENT VARIABLES
//////////////////////////////////////////////////////////////////////

Environment.SetEnvironmentVariable("CAKE_BUILD", "true");

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

string target = Argument("Target", "Validate");

//////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
//////////////////////////////////////////////////////////////////////

string artifacts = "./artifacts";
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

        DotNetTool("CycloneDX", new DotNetToolSettings
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
        DotNetClean(solution, new DotNetCleanSettings
        {
            Configuration = configuration
        });
    });

Task("Restore")
    .Does(() =>
    {
        DotNetRestore(solution);
    });

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        DotNetBuild(solution, new DotNetBuildSettings
        {
            Configuration = configuration,
            NoRestore = true
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

        DotNetTestSettings testSettings = new DotNetTestSettings
        {
            ArgumentCustomization = args => args
                .Append("--collect:\"XPlat Code Coverage\"")
                .Append("--logger:\"console;verbosity=minimal\"")
                .Append("--logger:\"trx;\"")
                .Append("--nologo"),
            Configuration = configuration,
            NoBuild = true
        };

        DotNetTest(solution, testSettings);

        string coverageHistory = $"{testArtifacts}/CoverageHistory";
        string coverageReports = $"{testArtifacts}/CoverageReports";
        string reportTypes = "Html;Cobertura;";
        string classFilters = "-Gogoproto*;-Logproto*;-Stats*";
        string fileFilters = "-*.g.cs;-*.generated.cs";

        DeleteDirectories(GetDirectories(coverageReports), deleteSettings);

        DotNetTool("reportgenerator", new DotNetToolSettings
        {
            ArgumentCustomization = args => args
                .Append($"-reports:\"./Tests/*.UnitTests/TestResults/*/coverage.cobertura.xml\"")
                .Append($"-reporttypes:\"{reportTypes}\"")
                .Append($"-targetdir:\"{coverageReports}/UnitTests\"")
                .Append($"-historydir:\"{coverageHistory}/UnitTests\"")
                .Append($"-title:\"{projectName} Unit Tests\"")
                .Append($"-verbosity:\"Error\"")
                .Append($"-classfilters:\"{classFilters}\"")
                .Append($"-filefilters:\"{fileFilters}")
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

        DotNetPackSettings settings = new DotNetPackSettings
        {
            Configuration = configuration,
            NoBuild = true,
            OutputDirectory = $"{buildArtifacts}"
        };

        DotNetPack("./Sources/LokiLoggingProvider", settings);
    });

Task("Validate")
    .IsDependentOn("Pack")
    .Does(() =>
    {
        FilePathCollection filePaths = GetFiles($"{buildArtifacts}/**/*.(nupkg|snupkg)");

        foreach (FilePath filePath in filePaths)
        {
            DotNetTool("validate", new DotNetToolSettings
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
