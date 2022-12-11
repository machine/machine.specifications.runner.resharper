using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications.Runner.ReSharper.Adapters.Discovery;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests.Adapters.Discovery;

[TestFixture]
public class MspecDiscoverySinkTests
{
    [Test]
    public async Task AddsSingleContext()
    {
        var context = new ContextElement("Namespace.ContextType", "subject", null);

        var specification1 = new SpecificationElement(context, "should_be");
        var specification2 = new SpecificationElement(context, "should_not_be");

        var sink = new MspecDiscoverySink(CancellationToken.None);
        sink.OnSpecification(specification1);
        sink.OnSpecification(specification2);
        sink.OnDiscoveryCompleted();

        var results = await sink.Elements;

        Assert.That(results.Length, Is.EqualTo(3));
        CollectionAssert.Contains(results, context);
        CollectionAssert.Contains(results, specification1);
        CollectionAssert.Contains(results, specification2);
    }

    [Test]
    public async Task AddsSingleBehavior()
    {
        var context = new ContextElement("Namespace.ContextType", "subject", null);

        var behavior = new BehaviorElement(context, "Namespace.ABehavior", "a_vehicle_that_is_started", null);

        var specification1 = new SpecificationElement(context, "should_be", null, behavior);
        var specification2 = new SpecificationElement(context, "should_not_be", null, behavior);

        var sink = new MspecDiscoverySink(CancellationToken.None);
        sink.OnSpecification(specification1);
        sink.OnSpecification(specification2);
        sink.OnDiscoveryCompleted();

        var results = await sink.Elements;

        Assert.That(results.Length, Is.EqualTo(4));
        CollectionAssert.Contains(results, context);
        CollectionAssert.Contains(results, behavior);
        CollectionAssert.Contains(results, specification1);
        CollectionAssert.Contains(results, specification2);
    }
}
