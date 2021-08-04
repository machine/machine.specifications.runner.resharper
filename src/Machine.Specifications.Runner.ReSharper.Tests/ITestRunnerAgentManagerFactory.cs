using JetBrains.ReSharper.UnitTestFramework.TestRunner;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    public interface ITestRunnerAgentManagerFactory
    {
        ITestRunnerAgentManager TestRunnerAgentManager { get; }
    }
}
