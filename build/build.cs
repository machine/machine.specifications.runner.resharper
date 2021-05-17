using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Xml.Linq;
using System.Xml.XPath;
using GlobExpressions;
using static Bullseye.Targets;
using static SimpleExec.Command;

public class build
{
    public static void Main(string[] args)
    {
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
                          "--configuration Debug " +
                          " /p:HostFullIdentifier= " +
                          $"/p:Version={version.SemVer} " +
                          $"/p:AssemblyVersion={version.AssemblySemVer} " +
                          $"/p:FileVersion={version.AssemblySemFileVer} " +
                          $"/p:InformationalVersion={version.InformationalVersion}");
        });

        Target("test", DependsOn("build"), () =>
        {
            Run("dotnet", "test --configuration Debug --no-restore --no-build");
        });

        Target("package", DependsOn("build", "test"), () =>
        {
            Run("dotnet", $"pack --configuration Debug --no-restore --no-build --output artifacts /p:Version={version.SemVer}");
        });

        Target("zip", DependsOn("package"), () =>
        {
            var artifactPath = Path.Combine("artifacts", "machine-specifications");
            var metaPath = Path.Combine(artifactPath, "META-INF");
            var archivePath = Path.Combine("artifacts", $"machine-specifications-{version.SemVer}.zip" );

            Directory.CreateDirectory(metaPath);

            var plugin = File.ReadAllText("plugin.xml")
                .Replace("${Version}", version.SemVer)
                .Replace("${SinceBuild}", waveVersion + ".0")
                .Replace("${UntilBuild}", waveVersion + ".*")
                .Replace("${ChangeNotes}", notes);

            File.WriteAllText(Path.Combine(metaPath, "plugin.xml"), plugin);

            var packages = Glob.Files("artifacts", "**/*.nupkg").ToArray();

            foreach (var package in packages)
            {
                var source = Path.Combine("artifacts", package);
                var target = Path.Combine(artifactPath, Path.GetFileName(package));

                File.Copy(source, target);
            }

            ZipFile.CreateFromDirectory(artifactPath, archivePath, CompressionLevel.Optimal, true);

            Console.WriteLine($"Created {archivePath}");
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
    }

    private static GitVersion GetGitVersion()
    {
        Run("dotnet", "tool restore");

        var value = Read("dotnet", "dotnet-gitversion");

        return JsonSerializer.Deserialize<GitVersion>(value);
    }

    private static string GetWaveVersion()
    {
        var value = GetXmlValue("SdkVersion");

        return $"{value.Substring(2,2)}{value.Substring(5,1)}";
    }

    private static string GetReleaseNotes()
    {
        return GetXmlValue("PackageReleaseNotes");
    }

    private static string GetXmlValue(string name)
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
}

public class GitVersion
{
    public string SemVer { get; set; }

    public string AssemblySemVer { get; set; }

    public string AssemblySemFileVer { get; set; }

    public string InformationalVersion { get; set; }

    public string PreReleaseTag { get; set; }
}
