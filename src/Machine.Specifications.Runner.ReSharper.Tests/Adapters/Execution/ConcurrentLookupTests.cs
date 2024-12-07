using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.ReSharper.Adapters.Execution;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests.Adapters.Execution;

public class ConcurrentLookupTests
{
    [Test]
    public void CanAddWithSameAggregateId()
    {
        var lookup = new ConcurrentLookup<ContextElement>();

        lookup.Add(new ContextElement("Type", "Subject", null));
        lookup.Add(new ContextElement("Type", "SubjectS", null));
    }

    [Test]
    public void CanTakeWhenUsingDuplicateIds()
    {
        var lookup = new ConcurrentLookup<ContextElement>();

        lookup.Add(new ContextElement("Type", "Subject", null));
        lookup.Add(new ContextElement("Type", "SubjectS", null));

        Assert.That(lookup.Take("Type"), Is.Not.Null);
        Assert.That(lookup.Take("Type"), Is.Not.Null);
    }

    [Test]
    public void ReturnsNullWhenAllTaken()
    {
        var lookup = new ConcurrentLookup<ContextElement>();

        lookup.Add(new ContextElement("Type", "Subject", null));
        lookup.Add(new ContextElement("Type", "SubjectS", null));

        Assert.That(lookup.Take("Type"), Is.Not.Null);
        Assert.That(lookup.Take("Type"), Is.Not.Null);
        Assert.That(lookup.Take("Type"), Is.Null);
    }
}
