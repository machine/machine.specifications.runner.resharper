using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.Util;
using NuGet.Frameworks;
using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests
{
    [MspecReferences]
    public abstract class MetadataTests : SingleProjectTest
    {
        private readonly TargetFrameworkId _framework = new TargetFrameworkId(new NuGetFramework(".NETFramework", Version.Parse("4.5")), true);

        private readonly MetadataElementCollector _collector = new MetadataElementCollector();

        public override ICollector Collector => _collector;

        protected override void WithFile(IProject project, string filename, Action<MspecContext> action)
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

                var context = new MspecContext(_collector);

                action(context);
            }
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
    }
}