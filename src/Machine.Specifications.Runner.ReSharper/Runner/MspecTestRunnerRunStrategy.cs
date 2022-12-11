using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DotNetCore;
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
        ITestRunnerHostSource testRunnerHostSource,
        MspecTestRunnerOrchestrator adapter,
        IUnitTestProjectArtifactResolver artifactResolver,
        DotNetCoreLaunchSettingsJsonProfileProvider launchSettingsProvider)
        : base(dataCollectorFactory, agentManagerHost.AgentManager, testRunnerHostSource, adapter, artifactResolver, launchSettingsProvider)
    {
    }
}
