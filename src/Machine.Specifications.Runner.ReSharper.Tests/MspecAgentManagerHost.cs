using JetBrains.Application.Components;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.TestRunner.Abstractions.Isolation;
using JetBrains.ReSharper.UnitTestFramework.TestRunner;
using Machine.Specifications.Runner.ReSharper.Runner;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    [SolutionComponent]
    public class MspecAgentManagerHost : IAgentManagerHost, IHideImplementation<AgentManagerHost>
    {
        public MspecAgentManagerHost(IAssemblyResolver assemblyResolver)
        {
            TestRunnerAgentManager = new MspecTestRunnerAgentManager(assemblyResolver);
        }

        public ITestRunnerAgentManager TestRunnerAgentManager { get; }
    }
}
