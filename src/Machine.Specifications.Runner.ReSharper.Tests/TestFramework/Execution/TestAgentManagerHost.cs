using JetBrains.Application.Components;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.TestRunner.Abstractions.Isolation;
using JetBrains.ReSharper.UnitTestFramework.Execution.TestRunner;
using Machine.Specifications.Runner.ReSharper.Runner;

namespace Machine.Specifications.Runner.ReSharper.Tests.TestFramework.Execution;

[SolutionComponent(Instantiation.ContainerAsyncPrimaryThread)]
public class TestAgentManagerHost(IAssemblyResolver assemblyResolver)
    : IAgentManagerHost, IHideImplementation<AgentManagerHost>
{
    public ITestRunnerAgentManager AgentManager { get; } = new AgentManager(assemblyResolver);
}
