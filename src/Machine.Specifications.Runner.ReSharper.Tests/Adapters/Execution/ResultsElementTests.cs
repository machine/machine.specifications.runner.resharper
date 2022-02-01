using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.ReSharper.Adapters.Execution;
using NSubstitute;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests.Adapters.Execution
{
    [TestFixture]
    public class ResultsElementTests
    {
        [Test]
        public void CannotSetResultIfThereAreChildren()
        {
            var specification = new ResultsElement(Substitute.For<IMspecElement>());
            var context = new ResultsElement(Substitute.For<IMspecElement>(), new[] {specification});

            context.Finished(true);

            Assert.False(context.IsSuccessful);
        }

        [Test]
        public void ResultIsUnsuccessfulIfNoChildren()
        {
            var context = new ResultsElement(Substitute.For<IMspecElement>());

            Assert.False(context.IsSuccessful);
        }

        [Test]
        public void CanSetResultOnChild()
        {
            var specification = new ResultsElement(Substitute.For<IMspecElement>());
            var context = new ResultsElement(Substitute.For<IMspecElement>(), new[] { specification });

            specification.Finished(true);

            Assert.True(context.IsSuccessful);
            Assert.True(specification.IsSuccessful);
        }
    }
}
