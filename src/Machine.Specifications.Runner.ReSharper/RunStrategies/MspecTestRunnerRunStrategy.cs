using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DotNetCore;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.ReSharper.UnitTestFramework.TestRunner;
using JetBrains.ReSharper.UnitTestFramework.TestRunner.DataCollection;

namespace Machine.Specifications.Runner.ReSharper.RunStrategies
{
    [SolutionComponent]
    public class MspecTestRunnerRunStrategy : TestRunnerRunStrategy
    {
        public MspecTestRunnerRunStrategy(
            IDataCollectorFactory dataCollectorFactory,
            ITestRunnerAgentManager agentManager,
            ITestRunnerHostSource testRunnerHostSource,
            MspecTestRunnerOrchestrator adapter,
            IUnitTestProjectArtifactResolver artifactResolver,
            DotNetCoreLaunchSettingsJsonProfileProvider launchSettingsProvider)
            : base(dataCollectorFactory, agentManager, testRunnerHostSource, adapter, artifactResolver, launchSettingsProvider)
        {
        }
    }
}
