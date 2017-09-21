using System.Linq;
using Machine.Specifications.ReSharperProvider;
using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests.Psi
{
    [TestFixture]
    public class ContextTests : PsiTests
    {
        [Test]
        public void EmptyClassIsNotAContext()
        {
            WithPsiFile("EmptyClass.cs", x => Assert.That(Type().IsContext(), Is.False));
        }

        [Test]
        public void AbstractClassIsNotAContext()
        {
            WithPsiFile("AbstractSpecs.cs", x => Assert.That(Type().IsContext(), Is.False));
        }

        [Test]
        public void BehaviorClassIsNotAContext()
        {
            WithPsiFile("SingleBehavior.cs", x => Assert.That(Type().IsContext(), Is.False));
        }

        [Test]
        public void ConcreteClassWithAbstractSpecsIsNotAContext()
        {
            WithPsiFile("ConcreteAndAbstractSpecs.cs", x => Assert.That(Type().IsContext(), Is.False));
        }

        [Test]
        public void ClassWithOneSpecIsAContext()
        {
            WithPsiFile("SingleSpec.cs", x => Assert.That(Type().IsContext(), Is.True));
        }

        [Test]
        public void ClassWithOneBehaviorSpecIsAContext()
        {
            WithPsiFile("SingleBehaviorSpec.cs", x => Assert.That(Type("Specs").IsContext(), Is.True));
        }

        [Test]
        public void ClassWithGenericFieldIsNotAContext()
        {
            WithPsiFile("ClassWithGenericField.cs", x => Assert.That(Type().IsContext(), Is.False));
        }

        [Test]
        public void GenericClassIsNotAContext()
        {
            WithPsiFile("GenericWithSpecs.cs", x => Assert.That(Type().IsContext(), Is.False));
        }
    }
}
