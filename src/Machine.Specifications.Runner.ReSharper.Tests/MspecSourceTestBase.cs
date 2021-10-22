using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Exploration.Daemon;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    public abstract class MspecSourceTestBase : UnitTestSourceTestBase
    {
        protected override IUnitTestExplorerFromFile FileExplorer => Solution.GetComponent<MspecTestExplorerFromFile>();

        protected override string GetIdString(IUnitTestElement element)
        {
            return $"{element.NaturalId.ProviderId}::{element.NaturalId.ProjectId}::{element.NaturalId.TestId}";
        }
    }
}
