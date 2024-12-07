using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.ReSharper.Adapters;
using Machine.Specifications.Runner.ReSharper.Adapters.Execution;
using Machine.Specifications.Runner.ReSharper.Tests.Fixtures;
using NSubstitute;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests.Adapters.Execution;

[TestFixture]
public class RunContextTests
{
    [Test]
    public void TaskIsNotReportedToSink()
    {
        var sink = Substitute.For<ITestExecutionSink>();

        var depot = new RemoteTaskDepot([
            RemoteTaskFixtures.Context,
            RemoteTaskFixtures.Behavior1,
            RemoteTaskFixtures.Specification1,
            RemoteTaskFixtures.Behavior1Specification1
        ]);

        var context = new RunContext(depot, sink);

        Assert.That(context.GetTask(ElementFixtures.Specification1), Is.Not.Null);
        sink.DidNotReceive().DynamicTestDiscovered(Arg.Any<RemoteTask>());
    }

    [Test]
    public void TaskNotInDepotIsReportedToSink()
    {
        var sink = Substitute.For<ITestExecutionSink>();

        var depot = new RemoteTaskDepot([
            RemoteTaskFixtures.Context
        ]);

        var context = new RunContext(depot, sink);

        Assert.That(context.GetTask(ElementFixtures.Specification1), Is.Not.Null);
        sink.Received().DynamicTestDiscovered(Arg.Any<RemoteTask>());
    }
}
