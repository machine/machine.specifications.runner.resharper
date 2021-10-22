using System.Linq;
using JetBrains.Application.Threading;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.ReSharper.UnitTestFramework.Exploration.Artifacts;
using JetBrains.ReSharper.UnitTestFramework.Exploration.Daemon;
using JetBrains.Util;
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

            using (var transaction = discoveryManager.BeginTransaction(source))
            {
                var observer = new DispositionObserverOnFile(transaction.Observer);

                FileExplorer.ProcessFile(file, observer, InterruptableReadActivity.Empty);

                DumpElements(transaction.Elements, projectFile.Name + ".source");
            }
        }

        private class DispositionObserverOnFile : IUnitTestElementObserverOnFile
        {
            private readonly IUnitTestElementObserver inner;

            public DispositionObserverOnFile(IUnitTestElementObserver inner)
            {
                this.inner = inner;
            }

            public IUnitTestElementSource Source => inner.Source;

            public T GetElementById<T>(string testId)
            {
                return inner.GetElementById<T>(testId);
            }

            public void OnUnitTestElement(IUnitTestElement element)
            {
                inner.OnUnitTestElement(element);
            }

            public void OnUnitTestElementDisposition(IUnitTestLikeElement element, TextRange navigationRange, TextRange containingRange)
            {
            }
        }
    }
}
