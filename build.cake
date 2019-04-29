using System.Net.Http;

#tool nuget:?package=GitVersion.CommandLine&version=4.0.0

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var pluginApiKey = Argument("pluginapikey", EnvironmentVariable("PLUGIN_API_KEY"));

//////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
//////////////////////////////////////////////////////////////////////
var version = "0.1.0";
var versionNumber = "0.1.0";
var waveVersion = string.Empty;
var changeNotes = string.Empty;
var isPreRelease = false;

var artifacts = Directory("./artifacts");
var solution = File("./Machine.Specifications.Runner.Resharper.sln");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////
Task("Clean")
    .Does(() => 
{
    CleanDirectories("./src/**/bin");
    CleanDirectories("./src/**/obj");
    CleanDirectory(artifacts);
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() => 
{
    DotNetCoreRestore(solution);
});

Task("Wave")
    .IsDependentOn("Clean")
    .Does(() =>
{
    var value = GetXmlValue("SdkVersion");

    waveVersion = $"{value.Substring(2,2)}{value.Substring(5,1)}";
});

Task("Notes")
    .IsDependentOn("Clean")
    .Does(() =>
{
    changeNotes = GetXmlValue("PackageReleaseNotes");
});

Task("Versioning")
    .IsDependentOn("Clean")
    .Does(() => 
{
    if (!BuildSystem.IsLocalBuild)
    {
        GitVersion(new GitVersionSettings
        {
            OutputType = GitVersionOutput.BuildServer
        });
    }

    var result = GitVersion(new GitVersionSettings
    {
        OutputType = GitVersionOutput.Json
    });

    version = result.NuGetVersion;
    versionNumber = result.MajorMinorPatch;
    isPreRelease = !string.IsNullOrEmpty(result.PreReleaseTag);
});

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Versioning")
    .IsDependentOn("Wave")
    .IsDependentOn("Notes")
    .Does(() => 
{
    CreateDirectory(artifacts);

    DotNetCoreBuild(solution, new DotNetCoreBuildSettings
    {
        Configuration = configuration,
        ArgumentCustomization = x => x
            .Append("/p:HostFullIdentifier={0}", "")
            .Append("/p:Version={0}", version)
            .Append("/p:AssemblyVersion={0}", versionNumber)
            .Append("/p:FileVersion={0}", versionNumber)
    });
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() => 
{
    DotNetCoreTest(solution, new DotNetCoreTestSettings
    {
        Configuration = configuration,
        NoBuild = true
    });
});

Task("Package")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .WithCriteria(() => !string.IsNullOrEmpty(waveVersion))
    .Does(() => 
{
    var path = artifacts + Directory("machine-specifications");
    var metaPath = path + Directory("META-INF");    
    
    CreateDirectory(metaPath);

    TransformTextFile("plugin.xml", "${", "}")
        .WithToken("Version", version)
        .WithToken("SinceBuild", waveVersion + ".0")
        .WithToken("UntilBuild", waveVersion + ".*")
        .WithToken("ChangeNotes", changeNotes)
        .Save(metaPath + File("plugin.xml"));
    
    DotNetCorePack(solution, new DotNetCorePackSettings
    {
        Configuration = configuration,
        OutputDirectory = path,
        NoBuild = true,
        ArgumentCustomization = x => x
            .Append("/p:Version={0}", version)
    });

    Zip(artifacts, artifacts + File($"machine-specifications-{version}.zip"));
});

Task("Publish")
    .IsDependentOn("Package")
    .WithCriteria(() => BuildSystem.IsRunningOnAppVeyor)
    .WithCriteria(() => AppVeyor.Environment.Repository.Tag.IsTag)
    .Does(() =>
{
    var packages = GetFiles("./artifacts/**/*.nupkg");
    var plugins = GetFiles("./artifacts/**/*.zip");

    foreach (var package in packages)
    {
        DotNetCoreNuGetPush(package.FullPath, new DotNetCoreNuGetPushSettings
        {
            Source = "https://plugins.jetbrains.com",
            ApiKey = pluginApiKey
        });
    }

    foreach (var plugin in plugins)
    {
        using (var client = new HttpClient())
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "https://plugins.jetbrains.com/plugin/uploadPlugin"))
            {
                var content = new MultipartFormDataContent
                {
                    { new StringContent("com.intellij.resharper.machine.specifications"), "xmlId" },
                    { new ByteArrayContent(System.IO.File.ReadAllBytes(plugin.FullPath)), "file", plugin.GetFilename().ToString() }
                };

                if (isPreRelease)
                {
                    content.Add(new StringContent("channel"), "Beta");
                }

                request.Content = content;
                request.Headers.Add("Authorization", $"Bearer {pluginApiKey}");

                client.SendAsync(request).Wait();
            }
        }
    }
});

//////////////////////////////////////////////////////////////////////
// HELPER METHODS
//////////////////////////////////////////////////////////////////////
string GetXmlValue(string name)
{
    var projects = GetFiles("./src/**/*.csproj");

    foreach (var project in projects)
    {
        var value = XmlPeek(project, $"/Project/PropertyGroup/{name}/text()", new XmlPeekSettings
        {
            SuppressWarning = true
        });

        if (!string.IsNullOrEmpty(value))
        {
            return value;
        }
    }

    return string.Empty;
}

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////
Task("Default")
    .IsDependentOn("Publish");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////
RunTarget(target);
