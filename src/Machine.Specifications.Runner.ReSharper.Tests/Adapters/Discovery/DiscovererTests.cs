using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.ReSharper.Adapters;
using Machine.Specifications.Runner.ReSharper.Adapters.Discovery;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.ReSharper.Tasks;
using NSubstitute;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests.Adapters.Discovery
{
    [TestFixture]
    public class DiscovererTests
    {
        [Test]
        public void UnreportedBehaviorsAreAddedToDepot()
        {
            var container = new TestContainer("assembly.dll", ShadowCopy.None);
            var request = new TestDiscoveryRequest(container);

            var controller = Substitute.For<IMspecController>();
            var sink = Substitute.For<ITestDiscoverySink>();
            var depot = new RemoteTaskDepot(GetReportedTasks().ToArray());
            var discoverer = new Discoverer(request, controller, sink, depot, CancellationToken.None);

            var elements = GetDiscoveredElements().ToArray();

            controller.Find(Arg.Do<IMspecDiscoverySink>(x => PopulateSink(x, elements)), Arg.Any<string>());

            discoverer.Discover();

            foreach (var element in elements)
            {
                Assert.NotNull(depot[element]);
            }
        }

        private void PopulateSink(IMspecDiscoverySink sink, ISpecificationElement[] elements)
        {
            foreach (var element in elements)
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
            yield return MspecSpecificationRemoteTask.ToServer("Namespace.Context", "should", null, null, null, null);
            yield return MspecBehaviorSpecificationRemoteTask.ToServer("Namespace.Context.behaves_like", "Namespace.Context", "behaves_like", null);
            yield return MspecSpecificationRemoteTask.ToServer("Namespace.Context", "should_behave", "Namespace.ABehavior", null, null, null);
        }
    }
}
