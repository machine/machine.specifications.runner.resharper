using System.Linq;
using Machine.Specifications.ReSharperProvider;
using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests.Reflection
{
    [TestFixture]
    public class SpecificationTests : PsiTests
    {
        [Test]
        public void SingleSpecIsValid()
        {
            WithFile("SingleSpec.cs", x => Assert.That(Field("is_true").IsSpecification(), Is.True));
        }

        [Test]
        public void BaseSpecsNotRetrieved()
        {
            WithFile("ConcreteAndAbstractSpecs.cs", x => Assert.That(Type("ConcreteClass").GetFields().Any(y => y.IsSpecification()), Is.False));
        }
    }
}
