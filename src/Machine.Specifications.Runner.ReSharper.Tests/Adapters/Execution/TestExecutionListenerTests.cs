using System;
using System.Threading;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.ReSharper.Adapters;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.ReSharper.Adapters.Execution;
using Machine.Specifications.Runner.ReSharper.Adapters.Listeners;
using Machine.Specifications.Runner.ReSharper.Tests.Fixtures;
using NSubstitute;
using NUnit.Framework;
using TestResult = JetBrains.ReSharper.TestRunner.Abstractions.Objects.TestResult;

namespace Machine.Specifications.Runner.ReSharper.Tests.Adapters.Execution
{
    [TestFixture]
    public class TestExecutionListenerTests
    {
        [Test]
        public void CanStartAndEndContext()
        {
            var depot = new RemoteTaskDepot(new RemoteTask[]
            {
                RemoteTaskFixtures.Context
            });
            var cache = new ElementCache(Array.Empty<ISpecificationElement>());

            var sink = Substitute.For<ITestExecutionSink>();
            var context = new RunContext(depot, sink);

            var listener = new TestExecutionListener(context, cache, CancellationToken.None);

            listener.OnContextStart(ElementFixtures.Context);
            listener.OnContextEnd(ElementFixtures.Context, string.Empty);

            sink.Received(1).TestStarting(RemoteTaskFixtures.Context);
            sink.Received(1).TestFinished(RemoteTaskFixtures.Context, Arg.Any<string>(), TestResult.Success);
        }

        [Test]
        public void CanStartAndEndSpecification()
        {
            var depot = new RemoteTaskDepot(new RemoteTask[]
            {
                RemoteTaskFixtures.Context,
                RemoteTaskFixtures.Specification1
            });
            var cache = new ElementCache(new ISpecificationElement[]
            {
                ElementFixtures.Specification1
            });

            var sink = Substitute.For<ITestExecutionSink>();
            var context = new RunContext(depot, sink);

            var listener = new TestExecutionListener(context, cache, CancellationToken.None);

            listener.OnContextStart(ElementFixtures.Context);
            listener.OnSpecificationStart(ElementFixtures.Specification1);
            listener.OnSpecificationEnd(ElementFixtures.Specification1, string.Empty, new TestRunResult(TestStatus.Passing));
            listener.OnContextEnd(ElementFixtures.Context, string.Empty);

            sink.Received(1).TestStarting(RemoteTaskFixtures.Context);
            sink.Received(1).TestStarting(RemoteTaskFixtures.Specification1);
            sink.Received(1).TestFinished(RemoteTaskFixtures.Specification1, Arg.Any<string>(), TestResult.Success);
            sink.Received(1).TestFinished(RemoteTaskFixtures.Context, Arg.Any<string>(), TestResult.Success);
        }

        [Test]
        public void CanStartAndEndBehaviorSpecification()
        {
            var depot = new RemoteTaskDepot(new RemoteTask[]
            {
                RemoteTaskFixtures.Context,
                RemoteTaskFixtures.Behavior1,
                RemoteTaskFixtures.Behavior1Specification1
            });
            var cache = new ElementCache(new ISpecificationElement[]
            {
                ElementFixtures.Behavior1Specification1
            });

            var sink = Substitute.For<ITestExecutionSink>();
            var context = new RunContext(depot, sink);

            var listener = new TestExecutionListener(context, cache, CancellationToken.None);

            listener.OnContextStart(ElementFixtures.Context);
            listener.OnBehaviorStart(ElementFixtures.Behavior1);
            listener.OnSpecificationStart(ElementFixtures.Behavior1Specification1);
            listener.OnSpecificationEnd(ElementFixtures.Behavior1Specification1, string.Empty, new TestRunResult(TestStatus.Passing));
            listener.OnBehaviorEnd(ElementFixtures.Behavior1, string.Empty);
            listener.OnContextEnd(ElementFixtures.Context, string.Empty);

            sink.Received(1).TestStarting(RemoteTaskFixtures.Context);
            sink.Received(1).TestStarting(RemoteTaskFixtures.Behavior1);
            sink.Received(1).TestStarting(RemoteTaskFixtures.Behavior1Specification1);
            sink.Received(1).TestFinished(RemoteTaskFixtures.Behavior1Specification1, Arg.Any<string>(), TestResult.Success);
            sink.Received(1).TestFinished(RemoteTaskFixtures.Behavior1, Arg.Any<string>(), TestResult.Success);
            sink.Received(1).TestFinished(RemoteTaskFixtures.Context, Arg.Any<string>(), TestResult.Success);
        }

        [Test]
        public void FailingSpecificationCausesContextToFail()
        {
            var depot = new RemoteTaskDepot(new RemoteTask[]
            {
                RemoteTaskFixtures.Context,
                RemoteTaskFixtures.Specification1,
                RemoteTaskFixtures.Specification2
            });
            var cache = new ElementCache(new ISpecificationElement[]
            {
                ElementFixtures.Specification1,
                ElementFixtures.Specification2
            });

            var sink = Substitute.For<ITestExecutionSink>();
            var context = new RunContext(depot, sink);

            var listener = new TestExecutionListener(context, cache, CancellationToken.None);

            listener.OnContextStart(ElementFixtures.Context);
            listener.OnSpecificationStart(ElementFixtures.Specification1);
            listener.OnSpecificationEnd(ElementFixtures.Specification1, string.Empty, new TestRunResult(TestStatus.Passing));
            listener.OnSpecificationStart(ElementFixtures.Specification2);
            listener.OnSpecificationEnd(ElementFixtures.Specification2, string.Empty, new TestRunResult(TestStatus.Failing));
            listener.OnContextEnd(ElementFixtures.Context, string.Empty);

            sink.Received(1).TestFinished(RemoteTaskFixtures.Specification1, Arg.Any<string>(), TestResult.Success);
            sink.Received(1).TestFinished(RemoteTaskFixtures.Specification2, Arg.Any<string>(), TestResult.Failed);
            sink.Received(1).TestFinished(RemoteTaskFixtures.Context, Arg.Any<string>(), TestResult.Failed);
        }

        [Test]
        public void FailingBehaviorSpecificationCausesContextAndBehaviorToFail()
        {
            var depot = new RemoteTaskDepot(new RemoteTask[]
            {
                RemoteTaskFixtures.Context,
                RemoteTaskFixtures.Behavior1,
                RemoteTaskFixtures.Behavior1Specification1,
                RemoteTaskFixtures.Behavior1Specification2
            });
            var cache = new ElementCache(new ISpecificationElement[]
            {
                ElementFixtures.Behavior1Specification1,
                ElementFixtures.Behavior1Specification2
            });

            var sink = Substitute.For<ITestExecutionSink>();
            var context = new RunContext(depot, sink);

            var listener = new TestExecutionListener(context, cache, CancellationToken.None);

            listener.OnContextStart(ElementFixtures.Context);
            listener.OnBehaviorStart(ElementFixtures.Behavior1);
            listener.OnSpecificationStart(ElementFixtures.Behavior1Specification1);
            listener.OnSpecificationEnd(ElementFixtures.Behavior1Specification1, string.Empty, new TestRunResult(TestStatus.Passing));
            listener.OnSpecificationStart(ElementFixtures.Behavior1Specification2);
            listener.OnSpecificationEnd(ElementFixtures.Behavior1Specification2, string.Empty, new TestRunResult(TestStatus.Failing));
            listener.OnBehaviorEnd(ElementFixtures.Behavior1, string.Empty);
            listener.OnContextEnd(ElementFixtures.Context, string.Empty);

            sink.Received(1).TestFinished(RemoteTaskFixtures.Behavior1Specification1, Arg.Any<string>(), TestResult.Success);
            sink.Received(1).TestFinished(RemoteTaskFixtures.Behavior1Specification2, Arg.Any<string>(), TestResult.Failed);
            sink.Received(1).TestFinished(RemoteTaskFixtures.Behavior1, Arg.Any<string>(), TestResult.Failed);
            sink.Received(1).TestFinished(RemoteTaskFixtures.Context, Arg.Any<string>(), TestResult.Failed);
        }
    }
}
