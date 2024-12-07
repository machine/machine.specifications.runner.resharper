using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;

namespace Machine.Specifications.Runner.ReSharper.Tests.TestFramework.Execution;

public class TestDiscoverySink(IMessageBroker broker) : ITestDiscoverySink
{
    public void TestsDiscovered(params RemoteTask[] tasks)
    {
        broker.TestsDiscovered(tasks);
    }
}
