using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.Execution.TestRunner;
using JetBrains.ReSharper.UnitTestFramework.Execution.TestRunner.DataCollection;
using JetBrains.ReSharper.UnitTestFramework.Exploration.Artifacts;

namespace Machine.Specifications.Runner.ReSharper.Runner;

[SolutionComponent(Instantiation.DemandAnyThreadUnsafe)]
public class MspecTestRunnerRunStrategy(
    IDataCollectorFactory dataCollectorFactory,
    IAgentManagerHost agentManagerHost,
    MspecTestRunnerOrchestrator adapter,
    IUnitTestProjectArtifactResolver artifactResolver)
    : TestRunnerRunStrategy(dataCollectorFactory, agentManagerHost.AgentManager, adapter, artifactResolver);
