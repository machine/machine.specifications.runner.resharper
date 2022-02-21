using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.ReSharper.Adapters;
using Machine.Specifications.Runner.ReSharper.Adapters.Discovery;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.ReSharper.Tests.Fixtures;
using Machine.Specifications.Runner.ReSharper.Tests.TestFramework;
using NSubstitute;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests.Adapters.Discovery
{
    [TestFixture]
    public class DiscovererTests : UnitTestElementTestBase
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
            yield return ElementFixtures.Specification1;
            yield return ElementFixtures.Behavior1Specification1;
            yield return ElementFixtures.Behavior1Specification2;
        }

        private IEnumerable<RemoteTask> GetReportedTasks()
        {
            yield return RemoteTaskFixtures.Context;
            yield return RemoteTaskFixtures.Specification1;
            yield return RemoteTaskFixtures.Behavior1;
            yield return RemoteTaskFixtures.Behavior1Specification1;
        }
    }
}
