using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;

namespace Machine.Specifications.Runner.ReSharper.Tests.TestFramework.Execution
{
    public class TestDiscoverySink : ITestDiscoverySink
    {
        private readonly IMessageBroker broker;

        public TestDiscoverySink(IMessageBroker broker)
        {
            this.broker = broker;
        }

        public void TestsDiscovered(params RemoteTask[] tasks)
        {
            broker.TestsDiscovered(tasks);
        }
    }
}
