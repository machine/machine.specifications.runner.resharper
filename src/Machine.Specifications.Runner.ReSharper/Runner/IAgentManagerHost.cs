using JetBrains.ReSharper.UnitTestFramework.TestRunner;

namespace Machine.Specifications.Runner.ReSharper.Runner
{
    public interface IAgentManagerHost
    {
        ITestRunnerAgentManager TestRunnerAgentManager { get; }
    }
}
