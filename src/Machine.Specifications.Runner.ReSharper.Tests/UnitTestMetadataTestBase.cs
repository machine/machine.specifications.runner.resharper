using JetBrains.Lifetimes;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.ReSharper.UnitTestFramework.Exploration.Artifacts;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    [TestFixture]
    [Category("Unit Test support")]
    public abstract class UnitTestMetadataTestBase : UnitTestElementDiscoveryTestBase
    {
        protected override void DoTest(Lifetime lifetime, IProject testProject)
        {
            var name = testProject.GetSubItems()[0].Location.Name;

            var testAssembly = GetVirtualTestDataFilePath(name);

            var resolve = new AssemblyResolverOnFolders();
            resolve.AddPath(testAssembly.Directory);

            using (WriteLockCookie.Create())
            {
                PrepareBeforeRun(testProject);
            }

            var discoveryManager = Solution.GetComponent<IUnitTestDiscoveryManager>();

            var source = new UnitTestElementSource(UnitTestElementOrigin.Artifact,
                new ExplorationTarget(
                    testProject,
                    GetTargetFrameworkId(),
                    new MspecTestProvider()));

            using (discoveryManager.BeginTransaction(source))
            {
                var observer = new TestUnitTestElementObserver(source);

                ExploreAssembly().ProcessArtifact(observer, lifetime).Wait(lifetime);

                DumpElements(observer.Elements, name + ".metadata");
            }
        }

        protected virtual void PrepareBeforeRun(IProject testProject)
        {
        }

        protected abstract IUnitTestExplorerFromArtifacts ExploreAssembly();
    }
}
