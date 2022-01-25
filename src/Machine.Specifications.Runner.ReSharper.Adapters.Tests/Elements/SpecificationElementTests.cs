using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using NSubstitute;
using Xunit;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Tests.Elements
{
    public class SpecificationElementTests
    {
        [Fact]
        public void SetsCorrectIdForSpecification()
        {
            var context = Substitute.For<IContextElement>();
            context.TypeName.Returns("Namespace.ContextType");

            var specification = new SpecificationElement(context, "should_be");

            Assert.Equal("Namespace.ContextType.should_be", specification.Id);
        }

        [Fact]
        public void SetsCorrectIdForBehaviorSpecification()
        {
            var context = Substitute.For<IContextElement>();
            context.TypeName.Returns("Namespace.ContextType");

            var behavior = Substitute.For<IBehaviorElement>();
            behavior.FieldName.Returns("a_vehicle_that_is_started");

            var specification = new SpecificationElement(context, "should_be", behavior);

            Assert.Equal("Namespace.ContextType.a_vehicle_that_is_started.should_be", specification.Id);
        }
    }
}
