using System.Linq;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
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
        var depot = new RemoteTaskDepot(new RemoteTask[]
        {
            RemoteTaskFixtures.Context
        });

        Assert.NotNull(depot[ElementFixtures.Context]);
    }

    [Test]
    public void CanGetSpecificationByElement()
    {
        var depot = new RemoteTaskDepot(new RemoteTask[]
        {
            RemoteTaskFixtures.Context,
            RemoteTaskFixtures.Specification1
        });

        Assert.NotNull(depot[ElementFixtures.Specification1]);
    }

    [Test]
    public void CanGetBehaviorByElement()
    {
        var depot = new RemoteTaskDepot(new RemoteTask[]
        {
            RemoteTaskFixtures.Context,
            RemoteTaskFixtures.Behavior1
        });

        Assert.NotNull(depot[ElementFixtures.Behavior1]);
    }

    [Test]
    public void CanGetBehaviorSpecificationByElement()
    {
        var depot = new RemoteTaskDepot(new RemoteTask[]
        {
            RemoteTaskFixtures.Context,
            RemoteTaskFixtures.Behavior1,
            RemoteTaskFixtures.Behavior1Specification1
        });

        Assert.NotNull(depot[ElementFixtures.Behavior1Specification1]);
    }

    [Test]
    public void CanGetBehaviorWhenThereIsSpecificationWithSameName()
    {
        var depot = new RemoteTaskDepot(new RemoteTask[]
        {
            RemoteTaskFixtures.Context,
            RemoteTaskFixtures.Behavior1,
            RemoteTaskFixtures.Specification1,
            RemoteTaskFixtures.Behavior1Specification1
        });

        Assert.NotNull(depot[ElementFixtures.Specification1]);
    }

    [Test]
    public void BoundElementsAreRunnable()
    {
        var depot = new RemoteTaskDepot(new RemoteTask[]
        {
            RemoteTaskFixtures.Context,
            RemoteTaskFixtures.Behavior1,
            RemoteTaskFixtures.Specification1,
            RemoteTaskFixtures.Behavior1Specification1
        });

        depot.Bind(ElementFixtures.Context, RemoteTaskFixtures.Context);
        depot.Bind(ElementFixtures.Behavior1, RemoteTaskFixtures.Behavior1);
        depot.Bind(ElementFixtures.Specification1, RemoteTaskFixtures.Specification1);
        depot.Bind(ElementFixtures.Behavior1Specification1, RemoteTaskFixtures.Behavior1Specification1);

        var selected = depot.GetTestsToRun().ToArray();

        CollectionAssert.Contains(selected, ElementFixtures.Specification1);
        CollectionAssert.Contains(selected, ElementFixtures.Behavior1Specification1);
    }
}
