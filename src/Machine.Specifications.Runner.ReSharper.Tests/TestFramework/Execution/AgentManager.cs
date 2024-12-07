using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Application.Components;
using JetBrains.ReSharper.TestRunner.Abstractions.Isolation;
using JetBrains.ReSharper.UnitTestFramework.Execution.TestRunner;

namespace Machine.Specifications.Runner.ReSharper.Tests.TestFramework.Execution;

public class AgentManager(IAssemblyResolver resolver) : ITestRunnerAgentManager
{
    public Task<ITestRunnerExecutionAgent> GetExecutionAgent(ITestRunnerExecutionContext context, CancellationToken ct)
    {
        var handlers = context.Container.GetComponents<IProvideTestRunnerAgentMessageHandlers>()
            .OrderBy(x => x.Priority)
            .SelectMany(x => x.GetMessageHandlers(context))
            .ToArray();

        var messageBroker = new MessageBroker(resolver, handlers);

        return Task.FromResult<ITestRunnerExecutionAgent>(new ExecutionAgent(context, messageBroker));
    }

    public Task<ITestRunnerDiscoveryAgent> GetDiscoveryAgent(ITestRunnerDiscoveryContext context, CancellationToken ct)
    {
        throw new NotSupportedException();
    }
}
