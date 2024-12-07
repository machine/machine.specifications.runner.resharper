using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.Execution.TestRunner;

namespace Machine.Specifications.Runner.ReSharper.Runner;

[SolutionComponent(Instantiation.ContainerAsyncPrimaryThread)]
public class AgentManagerHost(ITestRunnerAgentManager testRunnerAgentManager) : IAgentManagerHost
{
    public ITestRunnerAgentManager AgentManager { get; } = testRunnerAgentManager;
}
