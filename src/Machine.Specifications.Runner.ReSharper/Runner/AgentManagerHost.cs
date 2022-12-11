using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.Execution.TestRunner;

namespace Machine.Specifications.Runner.ReSharper.Runner;

[SolutionComponent]
public class AgentManagerHost : IAgentManagerHost
{
    public AgentManagerHost(ITestRunnerAgentManager testRunnerAgentManager)
    {
        AgentManager = testRunnerAgentManager;
    }

    public ITestRunnerAgentManager AgentManager { get; }
}
