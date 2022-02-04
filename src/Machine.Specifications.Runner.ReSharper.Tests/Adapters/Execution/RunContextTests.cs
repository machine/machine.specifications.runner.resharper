using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.ReSharper.Adapters;
using Machine.Specifications.Runner.ReSharper.Adapters.Execution;
using Machine.Specifications.Runner.ReSharper.Tests.Fixtures;
using NSubstitute;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests.Adapters.Execution
{
    [TestFixture]
    public class RunContextTests
    {
        [Test]
        public void TaskIsNotReportedToSink()
        {
            var sink = Substitute.For<ITestExecutionSink>();

            var depot = new RemoteTaskDepot(new RemoteTask[]
            {
                RemoteTaskFixtures.Context,
                RemoteTaskFixtures.Behavior,
                RemoteTaskFixtures.Specification,
                RemoteTaskFixtures.BehaviorSpecification
            });

            var context = new RunContext(depot, sink);

            Assert.NotNull(context.GetTask(ElementFixtures.Specification));
            sink.DidNotReceive().DynamicTestDiscovered(Arg.Any<RemoteTask>());
        }
    }
}
