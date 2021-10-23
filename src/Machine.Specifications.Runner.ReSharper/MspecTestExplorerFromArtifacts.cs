using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.Exploration.Artifacts;

namespace Machine.Specifications.Runner.ReSharper
{
    [SolutionComponent]
    public class MspecTestExplorerFromArtifacts : UnitTestExplorerFrom.Switching<MspecProviderSettings>
    {
        public MspecTestExplorerFromArtifacts(
            ISettingsStore settingsStore,
            MspecTestExplorerFromMetadata metadataExplorer,
            MspecTestExplorerFromTestRunner testRunnerExplorer)
            : base(settingsStore, x => x.TestDiscoveryFromArtifactsMethod, metadataExplorer, testRunnerExplorer)
        {
        }
    }
}
