using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.FeaturesTestFramework.UnitTesting;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.TestFramework;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;
using Machine.Specifications.ReSharperProvider;
using NuGet.Frameworks;
using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests
{
    [MspecReferences]
    public abstract class ReflectionWithSingleProject : BaseTestWithSingleProject
    {
        private readonly TargetFrameworkId _framework = new TargetFrameworkId(new NuGetFramework(".NETFramework", Version.Parse("4.5")), true);

        protected override string RelativeTestDataPath => "Reflection";

        protected MspecServiceProvider ServiceProvider =>
            Solution.GetComponent<MspecServiceProvider>();

        protected void With(Action action)
        {
            WithSingleProject("none.cs", (lifetime, solution, project) =>
            {
                RunGuarded(action);
            });
        }

        protected void With(Action<IProject> action)
        {
            WithSingleProject("none.cs", (lifetime, solution, project) =>
            {
                RunGuarded(() => action(project));
            });
        }

        protected void WithFile(string filename, Action<MspecContext> action)
        {
            Run(filename, (project, file, assembly) =>
            {
                var psiCollector = new MspecElementCollector();
                var metadataCollector = new MspecElementCollector();

                file.ProcessDescendants(psiCollector);
                metadataCollector.Explore(assembly);

                action(psiCollector.GetContext());
                action(metadataCollector.GetContext());
            });
        }

        protected void WithFile(string filename, Action<TestUnitTestElementObserver> action)
        {
            Run(filename, (project, file, assembly) =>
            {
                var psiObserver = GetPsiObserver(file, project);
                var metadataObserver = GetMetadataObserver(assembly, project);

                action(psiObserver);
                action(metadataObserver);
            });
        }

        private void Run(string filename, Action<IProject, IFile, IMetadataAssembly> action)
        {
            WithSingleProject(filename, (lifetime, solution, project) =>
            {
                RunGuarded(() =>
                {
                    var file = GetFile(project, filename);
                    var assembly = GetAssembly(filename, out var loader);

                    using (loader)
                        action(project, file, assembly);
                });
            });
        }

        protected UnitTestElementId CreateId(string id)
        {
            var factory = Solution.GetComponent<IUnitTestElementIdFactory>();

            return factory.Create("Provider", "Project", TargetFrameworkId.Default, id);
        }

        private IMetadataAssembly GetAssembly(string filename, out MetadataLoader loader)
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

            loader = new MetadataLoader(resolverOnFolders);
            var assembly = loader.TryLoadFrom(assemblyPath, x => true);

            Assert.That(assembly, Is.Not.Null, $"Cannot get metadata assembly: {filename}");

            return assembly;
        }

        private IFile GetFile(IProject project, string filename)
        {
            var file = project.GetAllProjectFiles(x => x.Name == filename)
                .FirstOrDefault();

            if (file == null || file.IsMissing)
                Assert.That(file, Is.Not.Null, $"Data file not found: {filename}");

            return file.ToSourceFile()?
                .GetTheOnlyPsiFile<CSharpLanguage>();
        }

        private TestUnitTestElementObserver GetMetadataObserver(IMetadataAssembly assembly, IProject project)
        {
            var serviceProvider = Solution.GetComponent<MspecServiceProvider>();
            var observer = new TestUnitTestElementObserver();
            var factory = new UnitTestElementFactory(serviceProvider, project, observer.TargetFrameworkId);

            var explorer = new MspecTestMetadataExplorer(factory, observer);
            explorer.ExploreAssembly(assembly, CancellationToken.None);

            return observer;
        }

        private TestUnitTestElementObserver GetPsiObserver(IFile file, IProject project)
        {
            var serviceProvider = Solution.GetComponent<MspecServiceProvider>();
            var observer = new TestUnitTestElementObserver();
            var factory = new UnitTestElementFactory(serviceProvider, project, observer.TargetFrameworkId);

            var explorer = new MspecPsiFileExplorer(factory, file, observer, () => false);
            file.ProcessDescendants(explorer);

            return observer;
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
