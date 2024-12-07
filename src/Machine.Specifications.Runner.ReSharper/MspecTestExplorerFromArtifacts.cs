using JetBrains.Application.Parts;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.Exploration.Artifacts;

namespace Machine.Specifications.Runner.ReSharper;

[SolutionComponent(Instantiation.ContainerAsyncPrimaryThread)]
public class MspecTestExplorerFromArtifacts(
    ISettingsStore settingsStore,
    MspecTestExplorerFromMetadata metadataExplorer,
    MspecTestExplorerFromTestRunner testRunnerExplorer)
    : UnitTestExplorerFrom.Switching<MspecProviderSettings>(settingsStore, x => x.TestDiscoveryFromArtifactsMethod, metadataExplorer, testRunnerExplorer);
