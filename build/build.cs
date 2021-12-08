using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using System.Xml.XPath;
using GlobExpressions;
using static Bullseye.Targets;
using static SimpleExec.Command;

var version = GetGitVersion();
var waveVersion = GetWaveVersion();
var notes = GetReleaseNotes();
var apiKey = Environment.GetEnvironmentVariable("JETBRAINS_API_KEY");

Target("clean", () =>
{
    Run("dotnet", "clean");

    if (Directory.Exists("artifacts"))
    {
        Directory.Delete("artifacts", true);
    }
});

Target("restore", DependsOn("clean"), () =>
{
    Run("dotnet", "restore");
});

Target("build", DependsOn("restore"), () =>
{
    Run("dotnet", "build " +
                  "--no-restore " +
                  "--configuration Release " +
                  " /p:HostFullIdentifier= " +
                  $"/p:Version={version.SemVer} " +
                  $"/p:AssemblyVersion={version.AssemblySemVer} " +
                  $"/p:FileVersion={version.AssemblySemFileVer} " +
                  $"/p:InformationalVersion={version.InformationalVersion}");
});

Target("test", DependsOn("build"), () =>
{
    Run("dotnet", "test --configuration Release --no-restore --no-build");
});

Target("package", DependsOn("build", "test"), () =>
{
    Run("dotnet", $"pack --configuration Release --no-restore --no-build --output artifacts /p:Version={version.SemVer}");
});

Target("zip", DependsOn("package"), () =>
{
    var artifactPath = Path.Combine("artifacts", "machine-specifications");
    var dotnetPath = Path.Combine(artifactPath, "dotnet");
    var libPath = Path.Combine(artifactPath, "lib");
    var metaPath = Path.Combine(libPath, "META-INF");
    var jarPath = Path.Combine(libPath, $"machine-specifications-{version.SemVer}.jar");
    var zipPath = Path.Combine("artifacts", $"machine-specifications-{version.SemVer}.zip");

    Directory.CreateDirectory(dotnetPath);
    Directory.CreateDirectory(metaPath);

    var nupkg = Glob.Files("artifacts", "*.nupkg").First();

    using var archive = ZipFile.OpenRead(Path.Combine("artifacts", nupkg));

    foreach (var entry in archive.Entries.Where(x => x.FullName.Contains("DotFiles")))
    {
        entry.ExtractToFile(Path.Combine(dotnetPath, entry.Name));
    }

    var plugin = File.ReadAllText("plugin.xml")
        .Replace("${Version}", version.SemVer)
        .Replace("${SinceBuild}", waveVersion + ".0")
        .Replace("${UntilBuild}", waveVersion + ".*")
        .Replace("${ChangeNotes}", notes);

    var icon = File.ReadAllBytes(Path.Combine("images", "pluginIcon.svg"));

    File.WriteAllText(Path.Combine(metaPath, "plugin.xml"), plugin);
    File.WriteAllText(Path.Combine(metaPath, "MANIFEST.MF"), "Manifest-Version: 1.0");
    File.WriteAllBytes(Path.Combine(metaPath, "pluginIcon.svg"), icon);

    ZipFile.CreateFromDirectory(metaPath, jarPath, CompressionLevel.Optimal, true, new UnixUTF8Encoding());

    Directory.Delete(metaPath, true);

    ZipFile.CreateFromDirectory(artifactPath, zipPath, CompressionLevel.Optimal, true, new UnixUTF8Encoding());

    Console.WriteLine($"Created {zipPath}");
});

Target("publish-nuget", DependsOn("package"), () =>
{
    var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");

    var packages = Glob.Files("artifacts", "*.nupkg")
        .ToArray();

    Run("dotnet", $"nuget add source https://nuget.pkg.github.com/machine/index.json -n github -u machine -p {githubToken} --store-password-in-clear-text");

    foreach (var package in packages)
    {
        var path = Path.Combine("artifacts", package);

        Run("dotnet", $"nuget push {path} --source https://plugins.jetbrains.com/ --api-key {apiKey}");
        Run("dotnet", $"nuget push {path} --source github");

        Console.WriteLine($"Published plugin {package} to JetBrains hub");
    }
});

Target("publish-zip", DependsOn("zip"), () =>
{
    using var client = new HttpClient();

    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

    var plugins = Glob.Files("artifacts", "*.zip");

    foreach (var file in plugins)
    {
        var filename = Path.GetFileName(file);

        var content = new MultipartFormDataContent
        {
            { new StringContent("com.intellij.resharper.machine.specifications"), "xmlId" },
            { new ByteArrayContent(File.ReadAllBytes(Path.Combine("artifacts", file))), "file", filename },
            { new StringContent(notes), "notes" }
        };

        if (!string.IsNullOrEmpty(version.PreReleaseTag))
        {
            content.Add(new StringContent("channel"), "Beta");
        }

        var response = client.PostAsync("https://plugins.jetbrains.com/plugin/uploadPlugin", content);
        response.Wait(TimeSpan.FromMinutes(10));
        response.Result.EnsureSuccessStatusCode();

        Console.WriteLine($"Published plugin {filename} to JetBrains hub");
    }
});

Target("publish", DependsOn("publish-nuget", "publish-zip"));

Target("default", DependsOn("zip"));

RunTargetsAndExit(args);

GitVersion GetGitVersion()
{
    Run("dotnet", "tool restore");

    var value = Read("dotnet", "dotnet-gitversion");

    return JsonSerializer.Deserialize<GitVersion>(value);
}

string GetWaveVersion()
{
    var value = GetXmlValue("SdkVersion");

    return $"{value.Substring(2,2)}{value.Substring(5,1)}";
}

string GetReleaseNotes()
{
    return GetXmlValue("PackageReleaseNotes");
}

string GetXmlValue(string name)
{
    var projects = Glob.Files(Environment.CurrentDirectory, "src/**/*.csproj");

    foreach (var project in projects)
    {
        var document = XDocument.Load(project);
        var node = document.XPathSelectElement($"/Project/PropertyGroup/{name}");

        if (!string.IsNullOrEmpty(node.Value))
        {
            return node.Value;
        }
    }

    return string.Empty;
}

public class GitVersion
{
    public string SemVer { get; set; }

    public string AssemblySemVer { get; set; }

    public string AssemblySemFileVer { get; set; }

    public string InformationalVersion { get; set; }

    public string PreReleaseTag { get; set; }
}

public class UnixUTF8Encoding : UTF8Encoding
{
    public override byte[] GetBytes(string s)
    {
        s = s.Replace("\\", "/");

        return base.GetBytes(s);
    }
}
