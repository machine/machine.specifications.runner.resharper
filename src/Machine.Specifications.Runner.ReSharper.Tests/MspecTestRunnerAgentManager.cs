using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Application.Components;
using JetBrains.ReSharper.TestRunner.Abstractions.Isolation;
using JetBrains.ReSharper.UnitTestFramework.TestRunner;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    public class MspecTestRunnerAgentManager : ITestRunnerAgentManager
    {
        private readonly IAssemblyResolver resolver;

        public MspecTestRunnerAgentManager(IAssemblyResolver resolver)
        {
            this.resolver = resolver;
        }

        public Task<ITestRunnerExecutionAgent> GetExecutionAgent(ITestRunnerExecutionContext context, CancellationToken ct)
        {
            var handlers = context.Container.GetComponents<IProvideTestRunnerAgentMessageHandlers>()
                .OrderBy(x => x.Priority)
                .SelectMany(x => x.GetMessageHandlers(context))
                .ToArray();

            var messageBroker = new MspecMessageBroker(resolver, handlers);

            return Task.FromResult<ITestRunnerExecutionAgent>(new MspecExecutionAgent(context, messageBroker));
        }

        public Task<ITestRunnerDiscoveryAgent> GetDiscoveryAgent(ITestRunnerDiscoveryContext context, CancellationToken ct)
        {
            throw new NotSupportedException();
        }
    }
}
