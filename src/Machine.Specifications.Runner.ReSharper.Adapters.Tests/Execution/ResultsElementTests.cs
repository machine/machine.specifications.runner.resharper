using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.ReSharper.Adapters.Execution;
using NSubstitute;
using Xunit;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Tests.Execution
{
    public class ResultsElementTests
    {
        [Fact]
        public void CannotSetResultIfThereAreChildren()
        {
            var specification = new ResultsElement(Substitute.For<IMspecElement>());
            var context = new ResultsElement(Substitute.For<IMspecElement>(), new[] {specification});

            context.SetResult(true);

            Assert.False(context.IsSuccessful);
        }

        [Fact]
        public void ResultIsUnsuccessfulIfNoChildren()
        {
            var context = new ResultsElement(Substitute.For<IMspecElement>());

            Assert.False(context.IsSuccessful);
        }

        [Fact]
        public void CanSetResultOnChild()
        {
            var specification = new ResultsElement(Substitute.For<IMspecElement>());
            var context = new ResultsElement(Substitute.For<IMspecElement>(), new[] { specification });

            specification.SetResult(true);

            Assert.True(context.IsSuccessful);
            Assert.True(specification.IsSuccessful);
        }
    }
}
