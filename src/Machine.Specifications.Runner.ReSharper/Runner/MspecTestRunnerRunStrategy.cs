using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DotNetCore;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.ReSharper.UnitTestFramework.TestRunner;
using JetBrains.ReSharper.UnitTestFramework.TestRunner.DataCollection;

namespace Machine.Specifications.Runner.ReSharper.Runner
{
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
}
