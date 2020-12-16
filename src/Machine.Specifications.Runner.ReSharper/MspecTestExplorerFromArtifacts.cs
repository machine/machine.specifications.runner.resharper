using System.Threading;
using System.Threading.Tasks;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.Util.Dotnet.TargetFrameworkIds;

namespace Machine.Specifications.Runner.ReSharper
{
    [SolutionComponent]
    public class MspecTestExplorerFromArtifacts : IUnitTestExplorerFromArtifacts
    {
        private readonly MspecTestExplorerFromMetadata metadataExplorer;

        public MspecTestExplorerFromArtifacts(MspecTestExplorerFromMetadata metadataExplorer)
        {
            this.metadataExplorer = metadataExplorer;

            Provider = metadataExplorer.Provider;
        }

        public IUnitTestProvider Provider { get; }

        public PertinenceResult IsSupported(IProject project, TargetFrameworkId targetFrameworkId)
        {
            return metadataExplorer.IsSupported(project, targetFrameworkId);
        }

        public Task<ExplorationResult> ProcessArtifact(IProjectArtifactUnitTestElementObserver observer, CancellationToken token)
        {
            return metadataExplorer.ProcessArtifact(observer, token);
        }
    }
}
