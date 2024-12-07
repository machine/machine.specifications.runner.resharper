using System.Linq;
using Machine.Specifications.Runner.ReSharper.Adapters;
using Machine.Specifications.Runner.ReSharper.Tests.Fixtures;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests.Adapters;

[TestFixture]
public class RemoteTaskDepotTests
{
    [Test]
    public void CanGetContextByElement()
    {
        var depot = new RemoteTaskDepot([
            RemoteTaskFixtures.Context
        ]);

        Assert.That(depot[ElementFixtures.Context], Is.Not.Null);
    }

    [Test]
    public void CanGetSpecificationByElement()
    {
        var depot = new RemoteTaskDepot([
            RemoteTaskFixtures.Context,
            RemoteTaskFixtures.Specification1
        ]);

        Assert.That(depot[ElementFixtures.Specification1], Is.Not.Null);
    }

    [Test]
    public void CanGetBehaviorByElement()
    {
        var depot = new RemoteTaskDepot([
            RemoteTaskFixtures.Context,
            RemoteTaskFixtures.Behavior1
        ]);

        Assert.That(depot[ElementFixtures.Behavior1], Is.Not.Null);
    }

    [Test]
    public void CanGetBehaviorSpecificationByElement()
    {
        var depot = new RemoteTaskDepot([
            RemoteTaskFixtures.Context,
            RemoteTaskFixtures.Behavior1,
            RemoteTaskFixtures.Behavior1Specification1
        ]);

        Assert.That(depot[ElementFixtures.Behavior1Specification1], Is.Not.Null);
    }

    [Test]
    public void CanGetBehaviorWhenThereIsSpecificationWithSameName()
    {
        var depot = new RemoteTaskDepot([
            RemoteTaskFixtures.Context,
            RemoteTaskFixtures.Behavior1,
            RemoteTaskFixtures.Specification1,
            RemoteTaskFixtures.Behavior1Specification1
        ]);

        Assert.That(depot[ElementFixtures.Specification1], Is.Not.Null);
    }

    [Test]
    public void BoundElementsAreRunnable()
    {
        var depot = new RemoteTaskDepot([
            RemoteTaskFixtures.Context,
            RemoteTaskFixtures.Behavior1,
            RemoteTaskFixtures.Specification1,
            RemoteTaskFixtures.Behavior1Specification1
        ]);

        depot.Bind(ElementFixtures.Context, RemoteTaskFixtures.Context);
        depot.Bind(ElementFixtures.Behavior1, RemoteTaskFixtures.Behavior1);
        depot.Bind(ElementFixtures.Specification1, RemoteTaskFixtures.Specification1);
        depot.Bind(ElementFixtures.Behavior1Specification1, RemoteTaskFixtures.Behavior1Specification1);

        var selected = depot.GetTestsToRun().ToArray();

        Assert.That(selected, Has.Member(ElementFixtures.Specification1));
        Assert.That(selected, Has.Member(ElementFixtures.Behavior1Specification1));
    }
}
