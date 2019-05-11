//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////
var target = Argument<string>("target", "Default");
var configuration = Argument<string>("configuration", "Debug");
var verbosity = Argument<string>("verbosity", "Minimal");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var solutionDir = Directory("./");
var solutionFile = solutionDir + File("./AppVeyor.Topshelf.Service/AppVeyor.Topshelf.Service.sln");


//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    var objDirs = GetDirectories($"./**/obj/**/{configuration}");
    var binDirs = GetDirectories($"./**/bin/**/{configuration}");
        
    var directories = binDirs + objDirs;
    CleanDirectories(directories);
});

Task("Restore-NuGet-Packages")
    .Does(() =>
{
    DotNetCoreRestore(solutionFile);
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    var settings = new DotNetCoreBuildSettings
    {
        NoRestore = true,
        Configuration = configuration
    };

    DotNetCoreBuild(solutionFile, settings);

    var publishSettings = new DotNetCorePublishSettings
    {         
        NoRestore = true,
         Configuration = configuration,
         OutputDirectory = "./publish/win-x64",
         Runtime = "win-x64"
    };

    DotNetCorePublish(solutionFile, publishSettings);

    EnsureDirectoryExists("./artifacts");
    Zip("./publish/win-x64", "./artifacts/appveyor.topshelf.service.zip");
});

Task("Rebuild")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore-NuGet-Packages")
    .IsDependentOn("Build");

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Clean");
  
//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
