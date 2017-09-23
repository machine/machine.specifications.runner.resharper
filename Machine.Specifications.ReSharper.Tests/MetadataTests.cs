using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Impl;
using JetBrains.ReSharper.FeaturesTestFramework.UnitTesting;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;
using Machine.Specifications.ReSharperProvider.Explorers;
using NuGet.Frameworks;

namespace Machine.Specifications.ReSharper.Tests
{
    [MspecReferences]
    //[TestNetFramework45]
    public abstract class MetadataTests : UnitTestMetadataTestBase
    {
        protected override string RelativeTestDataPath => "Metadata";

        private readonly TargetFrameworkId _framework = new TargetFrameworkId(new NuGetFramework(".NETFramework", Version.Parse("4.5")), true);

        protected void ExecuteTest(string filename)
        {
            var assembly = GetAssembly(filename);

            DoTestSolution(assembly);
        }

        protected override void ExploreAssembly(
            IProject testProject, 
            FileSystemPath assemblyPath, 
            MetadataLoader loader,
            IUnitTestElementsObserver observer)
        {
            var assemblyExplorer = Solution.GetComponent<AssemblyExplorer>();

            var explorer = new MSpecTestMetadataExplorer(assemblyExplorer);

            var assembly = loader.TryLoadFrom(assemblyPath, x => true);

            explorer.ExploreAssembly(testProject, assembly, observer, CancellationToken.None);
        }

        protected override void PrepareBeforeRun(IProject testProject)
        {
            var assemblies = GetProjectReferences();

            foreach (var assembly in assemblies)
            {
                var location = FileSystemPath.Parse(assembly);
                var reference = ProjectToAssemblyReference.CreateFromLocation(testProject, location);

                ((ProjectImpl) testProject).DoAddReference(reference);

                if (location.IsAbsolute && location.ExistsFile)
                {
                    var localCopy = TestDataPath2.Combine(location.Name);
                    location.CopyFile(localCopy, true);
                }
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