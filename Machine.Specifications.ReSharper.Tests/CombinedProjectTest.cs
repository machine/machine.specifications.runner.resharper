using System;
using System.Collections.Generic;
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
    public abstract class CombinedProjectTest : BaseTestWithSingleProject
    {
        private readonly TargetFrameworkId _framework = new TargetFrameworkId(new NuGetFramework(".NETFramework", Version.Parse("4.5")), true);

        private readonly MetadataElementCollector _collector = new MetadataElementCollector();
        private readonly PsiElementCollector _psiCollector = new PsiElementCollector();

        protected override string RelativeTestDataPath => "Files";

        public void WithFile(string filename, Action<MspecContext> action)
        {
            WithSingleProject(filename, (lifetime, solution, project) =>
            {
                Locks.ReentrancyGuard.Execute(GetType().Name, () =>
                {
                    _collector.Types.Clear();
                    _collector.Fields.Clear();

                    _psiCollector.Types.Clear();
                    _psiCollector.Fields.Clear();

                    using (ReadLockCookie.Create())
                    {
                        WithPsiFile(project, filename, action);
                        WithMetadataFile(filename, action);
                    }
                });
            });
        }

        private void WithMetadataFile(string filename, Action<MspecContext> action)
        {
            var compiledAssembly = GetAssembly(filename);
            var path = GetTestDataFilePath2(filename).Directory;

            var resolverOnFolders = new AssemblyResolverOnFolders();
            resolverOnFolders.AddPath(path);

            var assemblyPath = path.Combine(compiledAssembly);

            using (var loader = new MetadataLoader(resolverOnFolders))
            {
                var assembly = loader.TryLoadFrom(assemblyPath, x => true);

                Assert.That(assembly, Is.Not.Null, $"Cannot get metadata assembly: {filename}");

                _collector.Explore(assembly);

                action(new MspecContext(_collector));
            }
        }

        private void WithPsiFile(IProject project, string filename, Action<MspecContext> action)
        {
            var file = GetFile(project, filename);

            Assert.That(file, Is.Not.Null, $"PSI file not found: {filename}");

            file.ProcessDescendants(_psiCollector);

            action(new MspecContext(_psiCollector));
        }

        private string GetAssembly(string filename)
        {
            var source = GetTestDataFilePath2(filename);
            var assembly = source.ChangeExtension("dll");

            var references = GetProjectReferences();

            CompileUtil.CompileCs(source, assembly, references.ToArray());

            return assembly.Name;
        }

        private IEnumerable<string> GetProjectReferences()
        {
            return GetReferencedAssemblies(GetPlatformID(), _framework)
                .Where(x => x.Contains("Machine.Specifications"));
        }

        private IFile GetFile(IProject project, string filename)
        {
            return project.GetAllProjectFiles(x => x.Name == filename)
                .FirstOrDefault()?
                .ToSourceFile()?
                .GetTheOnlyPsiFile<CSharpLanguage>();
        }
    }
}