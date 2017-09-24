using Machine.Specifications.ReSharperProvider;
using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests.Reflection
{
    [TestFixture]
    public class ContextTests : CombinedProjectTest
    {
        [Test]
        public void EmptyClassIsNotAContext()
        {
            WithFile("EmptyClass.cs", x => Assert.That(x.Type().IsContext(), Is.False));
        }

        [Test]
        public void AbstractClassIsNotAContext()
        {
            WithFile("AbstractSpecs.cs", x => Assert.That(x.Type().IsContext(), Is.False));
        }

        [Test]
        public void BehaviorClassIsNotAContext()
        {
            WithFile("SingleBehavior.cs", x => Assert.That(x.Type().IsContext(), Is.False));
        }

        [Test]
        public void ConcreteClassWithAbstractSpecsIsNotAContext()
        {
            WithFile("ConcreteAndAbstractSpecs.cs", x => Assert.That(x.Type().IsContext(), Is.False));
        }

        [Test]
        public void ClassWithOneSpecIsAContext()
        {
            WithFile("SingleSpec.cs", x => Assert.That(x.Type().IsContext(), Is.True));
        }

        [Test]
        public void ClassWithOneBehaviorSpecIsAContext()
        {
            WithFile("SingleBehaviorSpec.cs", x => Assert.That(x.Type("Specs").IsContext(), Is.True));
        }

        [Test]
        public void ClassWithGenericFieldIsNotAContext()
        {
            WithFile("ClassWithGenericField.cs", x => Assert.That(x.Type().IsContext(), Is.False));
        }

        [Test]
        public void GenericClassIsNotAContext()
        {
            WithFile("GenericWithSpecs.cs", x => Assert.That(x.Type().IsContext(), Is.False));
        }
    }
}
