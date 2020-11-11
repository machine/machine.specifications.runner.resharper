using JetBrains.Application.Infra;
using JetBrains.Application.platforms;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DotNetCore;
using JetBrains.ReSharper.UnitTestFramework;
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
            IUnitTestAgentManager agentManager,
            IUnitTestResultManager resultManager,
            MspecTestRunnerOrchestrator orchestrator,
            DotNetCoreLaunchSettingsJsonProfileProvider launchSettings) 
            : base(platformManager, assemblyInfoDatabase, dataCollectorFactory, agentManager, resultManager, orchestrator, launchSettings)
        {
        }
    }
}
