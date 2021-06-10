using JetBrains.ProjectModel;
using JetBrains.ReSharper.FeaturesTestFramework.UnitTesting;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Exploration;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    public abstract class MspecMetadataTestBase : UnitTestMetadataTestBase
    {
        protected override IUnitTestExplorerFromArtifacts ExploreAssembly()
        {
            return Solution.GetComponent<MspecTestExplorerFromArtifacts>();
        }

        protected override string GetIdString(IUnitTestElement element)
        {
            return $"{element.Id.Provider.ID}::{element.Id.Project.GetPersistentID()}::{element.Id.Id}";
        }
    }
}
