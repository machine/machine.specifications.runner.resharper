using JetBrains.ProjectModel;
using JetBrains.ReSharper.TestRunner.Abstractions.Isolation;
using JetBrains.ReSharper.UnitTestFramework.TestRunner;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    [SolutionComponent]
    public class TestRunnerAgentManagerFactory : ITestRunnerAgentManagerFactory
    {
        public TestRunnerAgentManagerFactory(IAssemblyResolver resolver)
        {
            TestRunnerAgentManager = new MspecTestRunnerAgentManager(resolver);
        }

        public ITestRunnerAgentManager TestRunnerAgentManager { get; }
    }
}
