using System.Linq;
using JetBrains.Application.Threading;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.ReSharper.UnitTestFramework.Exploration.Artifacts;
using JetBrains.ReSharper.UnitTestFramework.Exploration.Daemon;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    [TestFixture]
    [Category("Unit Test support")]
    public abstract class UnitTestSourceTestBase : UnitTestElementDiscoveryTestBase
    {
        protected abstract IUnitTestExplorerFromFile FileExplorer { get; }

        protected override void DoTest(Lifetime lifetime, IProject project)
        {
            var projectFile = project.GetSubItems().OfType<IProjectFile>().First();
            var file = projectFile.GetPrimaryPsiFile();

            var discoveryManager = Solution.GetComponent<IUnitTestDiscoveryManager>();

            var source = new UnitTestElementSource(UnitTestElementOrigin.Source,
                new ExplorationTarget(
                    project,
                    GetTargetFrameworkId(),
                    new MspecTestProvider()));

            using (discoveryManager.BeginTransaction(source))
            {
                var observer = new TestUnitTestElementObserver(source);

                FileExplorer.ProcessFile(file, observer, InterruptableReadActivity.Empty);

                DumpElements(observer.Elements, projectFile.Name + ".source");
            }
        }
    }
}
