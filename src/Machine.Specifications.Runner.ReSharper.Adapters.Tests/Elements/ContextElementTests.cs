using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Xunit;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Tests.Elements
{
    public class ContextElementTests
    {
        [Fact]
        public void SetsCorrectId()
        {
            var context = new ContextElement("Namespace.ContextType", "subject");

            Assert.Equal("Namespace.ContextType", context.Id);
        }
    }
}
