using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Application.Components;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.UnitTestFramework.TestRunner;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    [SolutionComponent]
    public class MspecTestRunnerAgentManager : ITestRunnerAgentManager
    {
        private readonly IMessageBroker messageBroker;

        public MspecTestRunnerAgentManager(IMessageBroker messageBroker)
        {
            this.messageBroker = messageBroker;
        }

        public Task<ITestRunnerExecutionAgent> GetExecutionAgent(ITestRunnerExecutionContext context, CancellationToken ct)
        {
            var handlers = context.Container.GetComponents<IProvideTestRunnerAgentMessageHandlers>()
                .OrderBy(x => x.Priority)
                .ToArray();

            foreach (var handler in handlers)
            {
                var messageHandlers = handler.GetMessageHandlers(context);
            }

            return Task.FromResult<ITestRunnerExecutionAgent>(new MspecExecutionAgent(context, messageBroker));
        }

        public Task<ITestRunnerDiscoveryAgent> GetDiscoveryAgent(ITestRunnerDiscoveryContext context, CancellationToken ct)
        {
            throw new NotSupportedException();
        }
    }
}
