using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.ReSharper.Adapters.Discovery;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.ReSharper.Tasks;
using NSubstitute;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Tests.Discovery
{
    [TestFixture]
    public class DiscovererTests
    {
        [Test]
        public void UnreportedBehaviorsAreAddedToSink()
        {
            var container = new TestContainer("assembly.dll", ShadowCopy.None);
            var request = new TestDiscoveryRequest(container);

            var controller = Substitute.For<IMspecController>();
            var sink = Substitute.For<ITestDiscoverySink>();
            var depot = new RemoteTaskDepot(GetReportedTasks().ToArray());
            var discoverer = new Discoverer(request, controller, sink, depot, CancellationToken.None);

            controller.Find(Arg.Do<IMspecDiscoverySink>(PopulateSink), Arg.Any<string>());

            discoverer.Discover();

            sink.TestsDiscovered(Arg.Is<RemoteTask[]>(x => x)
        }

        private void PopulateSink(IMspecDiscoverySink sink)
        {
            foreach (var element in GetDiscoveredElements())
            {
                sink.OnSpecification(element);
            }

            sink.OnDiscoveryCompleted();
        }

        private IEnumerable<ISpecificationElement> GetDiscoveredElements()
        {
            var context = new ContextElement("Namespace.Context", string.Empty);
            var behavior = new BehaviorElement(context, "Namespace.ABehavior", "behaves_like");

            yield return new SpecificationElement(context, "should");
            yield return new SpecificationElement(context, "should_behave", behavior);
            yield return new SpecificationElement(context, "should_not_behave", behavior);
        }

        private IEnumerable<RemoteTask> GetReportedTasks()
        {
            yield return MspecContextRemoteTask.ToServer("Namespace.Context", null, null, null);
            yield return MspecContextSpecificationRemoteTask.ToServer("Namespace.Context", "should", null, null, null, null);
            yield return MspecBehaviorSpecificationRemoteTask.ToServer("Namespace.Context.behaves_like", "Namespace.Context", "behaves_like", null);
            yield return MspecContextSpecificationRemoteTask.ToServer("Namespace.Context", "should_behave", "Namespace.ABehavior", null, null, null);
        }
    }
}
