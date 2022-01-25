using System.Threading.Tasks;
using Machine.Specifications.Runner.ReSharper.Adapters.Discovery;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Xunit;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Tests.Discovery
{
    public class MspecDiscoverySinkTests
    {
        [Fact]
        public async Task AddsSingleContext()
        {
            var context = new ContextElement("Namespace.ContextType", "subject");

            var specification1 = new SpecificationElement(context, "should_be");
            var specification2 = new SpecificationElement(context, "should_not_be");

            var sink = new MspecDiscoverySink();
            sink.OnSpecification(specification1);
            sink.OnSpecification(specification2);
            sink.OnDiscoveryCompleted();

            var results = await sink.Elements;

            Assert.Equal(3, results.Length);
            Assert.Contains(context, results);
            Assert.Contains(specification1, results);
            Assert.Contains(specification2, results);
        }

        [Fact]
        public async Task AddsSingleBehavior()
        {
            var context = new ContextElement("Namespace.ContextType", "subject");

            var behavior = new BehaviorElement(context, "Namespace.ABehavior", "a_vehicle_that_is_started");

            var specification1 = new SpecificationElement(context, "should_be", behavior);
            var specification2 = new SpecificationElement(context, "should_not_be", behavior);

            var sink = new MspecDiscoverySink();
            sink.OnSpecification(specification1);
            sink.OnSpecification(specification2);
            sink.OnDiscoveryCompleted();

            var results = await sink.Elements;

            Assert.Equal(4, results.Length);
            Assert.Contains(context, results);
            Assert.Contains(behavior, results);
            Assert.Contains(specification1, results);
            Assert.Contains(specification2, results);
        }
    }
}
