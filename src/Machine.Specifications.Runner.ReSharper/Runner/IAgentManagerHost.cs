using JetBrains.ReSharper.UnitTestFramework.Execution.TestRunner;

namespace Machine.Specifications.Runner.ReSharper.Runner
{
    public interface IAgentManagerHost
    {
        ITestRunnerAgentManager AgentManager { get; }
    }
}
