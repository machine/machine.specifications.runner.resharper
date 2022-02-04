using System.Linq;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.ReSharper.Adapters;
using Machine.Specifications.Runner.ReSharper.Tests.Fixtures;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests.Adapters
{
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
                RemoteTaskFixtures.Specification
            });

            Assert.NotNull(depot[ElementFixtures.Specification]);
        }

        [Test]
        public void CanGetBehaviorByElement()
        {
            var depot = new RemoteTaskDepot(new RemoteTask[]
            {
                RemoteTaskFixtures.Context,
                RemoteTaskFixtures.Behavior
            });

            Assert.NotNull(depot[ElementFixtures.Behavior]);
        }

        [Test]
        public void CanGetBehaviorSpecificationByElement()
        {
            var depot = new RemoteTaskDepot(new RemoteTask[]
            {
                RemoteTaskFixtures.Context,
                RemoteTaskFixtures.Behavior,
                RemoteTaskFixtures.BehaviorSpecification
            });

            Assert.NotNull(depot[ElementFixtures.BehaviorSpecification]);
        }

        [Test]
        public void CanGetBehaviorWhenThereIsSpecificationWithSameName()
        {
            var depot = new RemoteTaskDepot(new RemoteTask[]
            {
                RemoteTaskFixtures.Context,
                RemoteTaskFixtures.Behavior,
                RemoteTaskFixtures.Specification,
                RemoteTaskFixtures.BehaviorSpecification
            });

            Assert.NotNull(depot[ElementFixtures.Specification]);
        }

        [Test]
        public void BoundElementsAreSelected()
        {
            var depot = new RemoteTaskDepot(new RemoteTask[]
            {
                RemoteTaskFixtures.Context,
                RemoteTaskFixtures.Behavior,
                RemoteTaskFixtures.Specification,
                RemoteTaskFixtures.BehaviorSpecification
            });

            depot.Bind(ElementFixtures.Context, RemoteTaskFixtures.Context);
            depot.Bind(ElementFixtures.Behavior, RemoteTaskFixtures.Behavior);
            depot.Bind(ElementFixtures.Specification, RemoteTaskFixtures.Specification);
            depot.Bind(ElementFixtures.BehaviorSpecification, RemoteTaskFixtures.BehaviorSpecification);

            var selected = depot.GetSelection().ToArray();

            CollectionAssert.Contains(selected, ElementFixtures.Context);
            CollectionAssert.Contains(selected, ElementFixtures.Behavior);
            CollectionAssert.Contains(selected, ElementFixtures.Specification);
            CollectionAssert.Contains(selected, ElementFixtures.BehaviorSpecification);
        }
    }
}
