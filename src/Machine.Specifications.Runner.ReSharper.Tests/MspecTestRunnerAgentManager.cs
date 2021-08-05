using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Application.Components;
using JetBrains.ReSharper.TestRunner.Abstractions.Isolation;
using JetBrains.ReSharper.UnitTestFramework.Launch;
using JetBrains.ReSharper.UnitTestFramework.TestRunner;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    public class MspecTestRunnerAgentManager : ITestRunnerAgentManager
    {
        private readonly IAssemblyResolver resolver;

        private readonly IMessageSink sink;

        public MspecTestRunnerAgentManager(IAssemblyResolver resolver, IMessageSink sink)
        {
            this.resolver = resolver;
            this.sink = sink;
        }

        public Task<ITestRunnerExecutionAgent> GetExecutionAgent(ITestRunnerExecutionContext context, CancellationToken ct)
        {
            var handlers = context.Container.GetComponents<IProvideTestRunnerAgentMessageHandlers>()
                .OrderBy(x => x.Priority)
                .SelectMany(x => x.GetMessageHandlers(context))
                .ToArray();

            var messageBroker = new MspecMessageBroker(resolver, handlers, sink);

            return Task.FromResult<ITestRunnerExecutionAgent>(new MspecExecutionAgent(context, messageBroker));
        }

        public Task<ITestRunnerDiscoveryAgent> GetDiscoveryAgent(ITestRunnerDiscoveryContext context, CancellationToken ct)
        {
            throw new NotSupportedException();
        }
    }
}
