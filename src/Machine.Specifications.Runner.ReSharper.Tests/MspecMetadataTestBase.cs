using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Exploration.Artifacts;

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
            return $"{element.NaturalId.ProviderId}::{element.NaturalId.ProjectId}::{element.NaturalId.TestId}";
        }
    }
}
