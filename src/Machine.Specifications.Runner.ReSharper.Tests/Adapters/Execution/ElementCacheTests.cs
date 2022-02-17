using System.Linq;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.ReSharper.Adapters.Execution;
using Machine.Specifications.Runner.ReSharper.Tests.Fixtures;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests.Adapters.Execution
{
    public class ElementCacheTests
    {
        [Test]
        public void CanGetBehaviorType()
        {
            var cache = new ElementCache(new ISpecificationElement[]
            {
                ElementFixtures.Specification1,
                ElementFixtures.Behavior1Specification1,
                ElementFixtures.Behavior2Specification1
            });

            Assert.IsTrue(cache.IsBehavior(ElementFixtures.Behavior1.TypeName));
            Assert.IsTrue(cache.IsBehavior(ElementFixtures.Behavior2.TypeName));
            Assert.IsFalse(cache.IsBehavior(ElementFixtures.Context.TypeName));
        }

        [Test]
        public void CanGetBehaviors()
        {
            var cache = new ElementCache(new ISpecificationElement[]
            {
                ElementFixtures.Specification1,
                ElementFixtures.Behavior1Specification1,
                ElementFixtures.Behavior1Specification2,
                ElementFixtures.Behavior2Specification1,
            });

            var behaviors = cache.GetBehaviors(ElementFixtures.Context).ToArray();

            Assert.That(behaviors, Has.Length.EqualTo(2));
            Assert.That(behaviors, Contains.Item(ElementFixtures.Behavior1));
            Assert.That(behaviors, Contains.Item(ElementFixtures.Behavior2));
        }

        [Test]
        public void CanGetSpecificationsByContext()
        {
            var cache = new ElementCache(new ISpecificationElement[]
            {
                ElementFixtures.Specification1,
                ElementFixtures.Behavior1Specification1,
                ElementFixtures.Behavior1Specification2,
                ElementFixtures.Behavior2Specification1,
            });

            var specifications = cache.GetSpecifications(ElementFixtures.Context).ToArray();

            Assert.That(specifications, Has.Length.EqualTo(4));
            Assert.That(specifications, Contains.Item(ElementFixtures.Specification1));
            Assert.That(specifications, Contains.Item(ElementFixtures.Behavior1Specification1));
            Assert.That(specifications, Contains.Item(ElementFixtures.Behavior1Specification2));
            Assert.That(specifications, Contains.Item(ElementFixtures.Behavior2Specification1));
        }

        [Test]
        public void CanGetSpecificationsByBehavior()
        {
            var cache = new ElementCache(new ISpecificationElement[]
            {
                ElementFixtures.Specification1,
                ElementFixtures.Behavior1Specification1,
                ElementFixtures.Behavior1Specification2,
                ElementFixtures.Behavior2Specification1
            });

            var specifications1 = cache.GetSpecifications(ElementFixtures.Behavior1).ToArray();
            var specifications2 = cache.GetSpecifications(ElementFixtures.Behavior2).ToArray();

            Assert.That(specifications1, Has.Length.EqualTo(2));
            Assert.That(specifications1, Contains.Item(ElementFixtures.Behavior1Specification1));
            Assert.That(specifications1, Contains.Item(ElementFixtures.Behavior1Specification2));

            Assert.That(specifications2, Has.Length.EqualTo(1));
            Assert.That(specifications2, Contains.Item(ElementFixtures.Behavior2Specification1));
        }
    }
}
