using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using NSubstitute;
using Xunit;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Tests.Elements
{
    public class BehaviorElementTests
    {
        [Fact]
        public void SetsCorrectId()
        {
            var context = Substitute.For<IContextElement>();
            context.TypeName.Returns("Namespace.ContextType");

            var element = new BehaviorElement(context, "Namespace.BehaviorType", "a_vehicle_that_is_started");

            Assert.Equal("Namespace.ContextType.a_vehicle_that_is_started", element.Id);
        }
    }
}
