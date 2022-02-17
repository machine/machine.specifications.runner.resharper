using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using NSubstitute;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests.Adapters.Elements
{
    [TestFixture]
    public class BehaviorElementTests
    {
        [Test]
        public void SetsCorrectId()
        {
            var context = Substitute.For<IContextElement>();
            context.TypeName.Returns("Namespace.ContextType");

            var element = new BehaviorElement(context, "Namespace.BehaviorType", "a_vehicle_that_is_started");

            Assert.That(element.Id, Is.EqualTo("Namespace.ContextType.a_vehicle_that_is_started"));
        }

        [Test]
        public void SetsCorrectAggregateId()
        {
            var context = Substitute.For<IContextElement>();
            context.TypeName.Returns("Namespace.ContextType");

            var element = new BehaviorElement(context, "Namespace.BehaviorType", "a_vehicle_that_is_started");

            Assert.That(element.AggregateId, Is.EqualTo("Namespace.ContextType.Namespace.BehaviorType"));
        }
    }
}
