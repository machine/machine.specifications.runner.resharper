using System;
using System.Threading.Tasks;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Isolation;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    public class MspecTestRemoteAgent : IMessageHandler<RemoteAgentInitializationRequest>, IAsyncMessageHandler<TestRunRequest>
    {
        private readonly IMessageBroker broker;

        private readonly IAssemblyResolver resolver;

        private TestAdapterInfo loader;

        public MspecTestRemoteAgent(IMessageBroker broker, IAssemblyResolver resolver)
        {
            this.broker = broker;
            this.resolver = resolver;
        }

        public void Execute(RemoteAgentInitializationRequest message)
        {
            if (message.TestAdapterLoader is TestAdapterInfo info)
            {
                loader = info;
            }
        }

        public Task Execute(TestRunRequest message)
        {
            var executor = CreateTestExecutor();

            executor.RunTests(message, new TestDiscoverySink(broker), new TestExecutionSink(broker));

            return Task.CompletedTask;
        }

        private ITestExecutor CreateTestExecutor()
        {
            var type = resolver
                .LoadFrom(loader.Executor.Assembly)
                .GetType(loader.Executor.TypeName);

            return (ITestExecutor) Activator.CreateInstance(type);
        }
    }
}
