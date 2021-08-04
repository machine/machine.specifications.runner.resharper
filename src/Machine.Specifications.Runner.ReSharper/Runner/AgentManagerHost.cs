using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.TestRunner;

namespace Machine.Specifications.Runner.ReSharper.Runner
{
    [SolutionComponent]
    public class AgentManagerHost : IAgentManagerHost
    {
        public AgentManagerHost(ITestRunnerAgentManager testRunnerAgentManager)
        {
            TestRunnerAgentManager = testRunnerAgentManager;
        }

        public ITestRunnerAgentManager TestRunnerAgentManager { get; }
    }
}
