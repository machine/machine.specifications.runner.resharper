using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.Execution.TestRunner;
using JetBrains.ReSharper.UnitTestFramework.Execution.TestRunner.DataCollection;
using JetBrains.ReSharper.UnitTestFramework.Exploration.Artifacts;

namespace Machine.Specifications.Runner.ReSharper.Runner;

[SolutionComponent]
public class MspecTestRunnerRunStrategy : TestRunnerRunStrategy
{
    public MspecTestRunnerRunStrategy(
        IDataCollectorFactory dataCollectorFactory,
        IAgentManagerHost agentManagerHost,
        MspecTestRunnerOrchestrator adapter,
        IUnitTestProjectArtifactResolver artifactResolver)
        : base(dataCollectorFactory, agentManagerHost.AgentManager, adapter, artifactResolver)
    {
    }
}
