using JetBrains.ProjectModel;
using JetBrains.ReSharper.FeaturesTestFramework.UnitTesting;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Exploration;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    public abstract class MspecSourceTestBase : UnitTestSourceTestBase
    {
        protected override IUnitTestExplorerFromFile FileExplorer => Solution.GetComponent<MspecTestExplorerFromFile>();

        protected override string GetIdString(IUnitTestElement element)
        {
            return $"{element.Id.Provider.ID}::{element.Id.Project.GetPersistentID()}::{element.Id.Id}";
        }
    }
}
