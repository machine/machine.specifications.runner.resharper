using System.Linq;
using JetBrains.Application.Threading;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.ReSharper.UnitTestFramework.Exploration.Artifacts;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests.TestFramework
{
    [TestFixture]
    [Category("Unit Test support")]
    public abstract class UnitTestSourceTestBase : UnitTestElementDiscoveryTestBase
    {
        protected override string GetIdString(IUnitTestElement element)
        {
            return $"{element.NaturalId.ProviderId}::{element.NaturalId.ProjectId}::{element.NaturalId.TestId}";
        }

        protected override void DoTest(Lifetime lifetime, IProject project)
        {
            var projectFile = project.GetSubItems().OfType<IProjectFile>().First();
            var file = projectFile.GetPrimaryPsiFile();

            var discoveryManager = Solution.GetComponent<IUnitTestDiscoveryManager>();
            var fileExplorer = Solution.GetComponent<MspecTestExplorerFromFile>();

            var source = new UnitTestElementSource(UnitTestElementOrigin.Source,
                new ExplorationTarget(
                    project,
                    GetTargetFrameworkId(),
                    new MspecTestProvider()));

            using (var transaction = discoveryManager.BeginTransaction(source))
            {
                var observer = new TestElementObserverOnFile(transaction.Observer);

                fileExplorer.ProcessFile(file, observer, InterruptableReadActivity.Empty);

                DumpElements(transaction.Elements, projectFile.Name + ".source");
            }
        }
    }
}
