using JetBrains.Application.Infra;
using JetBrains.Application.platforms;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.ReSharper.UnitTestFramework.TestRunner;
using JetBrains.ReSharper.UnitTestFramework.TestRunner.DataCollection;
using IUnitTestAgentManager = JetBrains.ReSharper.UnitTestFramework.TestRunner.IUnitTestAgentManager;

namespace Machine.Specifications.Runner.ReSharper.RunStrategies
{
    [SolutionComponent]
    public class MspecTestRunnerRunStrategy : TestRunnerRunStrategy
    {
        public MspecTestRunnerRunStrategy(
            IPlatformManager platformManager,
            IAssemblyInfoDatabase assemblyInfoDatabase,
            IDataCollectorFactory dataCollectorFactory,
            IUnitTestProjectArtifactResolver artifactResolver,
            IUnitTestAgentManager agentManager,
            IUnitTestResultManager resultManager,
            MspecTestRunnerOrchestrator orchestrator) 
            : base(platformManager, assemblyInfoDatabase, dataCollectorFactory, artifactResolver, agentManager, resultManager, orchestrator)
        {
        }
    }
}
