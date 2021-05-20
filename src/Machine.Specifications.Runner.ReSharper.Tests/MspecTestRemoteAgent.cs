using System.Threading.Tasks;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    public class MspecTestRemoteAgent : IMessageHandler<RemoteAgentInitializationRequest>, IAsyncMessageHandler<TestRunRequest>
    {
        private readonly IMessageBroker broker;

        private readonly ITestAdapterLoadContextFactory contextFactory;

        public MspecTestRemoteAgent(IMessageBroker broker, ITestAdapterLoadContextFactory contextFactory)
        {
            this.broker = broker;
            this.contextFactory = contextFactory;
        }

        public void Execute(RemoteAgentInitializationRequest message)
        {
            if (message.TestAdapterLoader is TestAdapterInfo info)
            {
                contextFactory.SetTestAdapterLoader(info);
            }
        }

        public Task Execute(TestRunRequest message)
        {
            var context = contextFactory.Initialize();
            var executor = context.InitializeTestExecutor();

            executor.RunTests(message, new TestDiscoverySink(broker), new TestExecutionSink(broker));

            return Task.CompletedTask;
        }
    }
}
