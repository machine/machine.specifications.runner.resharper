using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.ReSharper.TestFramework;
using JetBrains.Util;
using NuGet.Frameworks;
using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests
{
    [MspecReferences]
    public abstract class SingleProjectTest : BaseTestWithSingleProject
    {
        private readonly TargetFrameworkId _framework = new TargetFrameworkId(new NuGetFramework(".NETFramework", Version.Parse("4.5")), true);

        private readonly MspecElementCollector _collector = new MspecElementCollector();

        protected override string RelativeTestDataPath => "Reflection";

        protected void WithFile(string filename, Action<MspecContext> action)
        {
            WithSingleProject(filename, (lifetime, solution, project) =>
            {
                Locks.ReentrancyGuard.Execute(GetType().Name, () =>
                {
                    _collector.Types.Clear();
                    _collector.Fields.Clear();

                    using (ReadLockCookie.Create())
                    {
                        WithPsiFile(project, filename, action);
                        WithMetadataFile(filename, action);
                    }
                });
            });
        }

        private void WithPsiFile(IProject project, string filename, Action<MspecContext> action)
        {
            var file = GetFile(project, filename);

            Assert.That(file, Is.Not.Null, $"Data file not found: {filename}");

            _collector.Reset();
            file.ProcessDescendants(_collector);

            action(new MspecContext(_collector));
        }

        private void WithMetadataFile(string filename, Action<MspecContext> action)
        {
            var references = GetProjectReferences()
                .ToArray();

            var compiledAssembly = GetAssembly(filename, references);
            var path = GetTestDataFilePath2(filename).Directory;

            var referencePaths = references
                .Select(Path.GetDirectoryName)
                .Select(x => FileSystemPath.Parse(x));

            var resolverOnFolders = new AssemblyResolverOnFolders();
            resolverOnFolders.AddPath(path);

            foreach (var referencePath in referencePaths)
                resolverOnFolders.AddPath(referencePath);

            var assemblyPath = path.Combine(compiledAssembly);

            using (var loader = new MetadataLoader(resolverOnFolders))
            {
                var assembly = loader.TryLoadFrom(assemblyPath, x => true);

                Assert.That(assembly, Is.Not.Null, $"Cannot get metadata assembly: {filename}");

                _collector.Reset();
                _collector.Explore(assembly);

                action(new MspecContext(_collector));
            }
        }

        private IFile GetFile(IProject project, string filename)
        {
            return project.GetAllProjectFiles(x => x.Name == filename)
                .FirstOrDefault()?
                .ToSourceFile()?
                .GetTheOnlyPsiFile<CSharpLanguage>();
        }

        private string GetAssembly(string filename, string[] references)
        {
            var source = GetTestDataFilePath2(filename);
            var assembly = source.ChangeExtension("dll");

            CompileUtil.CompileCs(source, assembly, references);

            return assembly.Name;
        }

        private IEnumerable<string> GetProjectReferences()
        {
            return GetReferencedAssemblies(GetPlatformID(), _framework)
                .Where(x => x.Contains("Machine.Specifications"));
        }
    }
}
