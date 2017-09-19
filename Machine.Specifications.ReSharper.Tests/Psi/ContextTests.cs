using System.Linq;
using Machine.Specifications.ReSharperProvider;
using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests.Psi
{
    [TestFixture]
    public class ContextTests : PsiTests
    {
        [Test]
        public void StructsAreIgnored()
        {
            WithPsiFile("EmptyStruct.cs", x =>
            {
                CollectionAssert.IsEmpty(Classes);
                CollectionAssert.IsEmpty(Fields);
            });
        }

        [Test]
        public void InterfacesAreIgnored()
        {
            WithPsiFile("EmptyInterface.cs", x =>
            {
                CollectionAssert.IsEmpty(Classes);
                CollectionAssert.IsEmpty(Fields);
            });
        }

        [Test]
        public void EmptyClassIsNotAContext()
        {
            WithPsiFile("EmptyClass.cs", x =>
            {
                var type = Classes.FirstOrDefault()?.AsTypeInfo();

                Assert.That(type, Is.Not.Null);
                Assert.That(type.IsContext(), Is.False);
            });
        }

        [Test]
        public void AbstractClassIsNotAContext()
        {
            WithPsiFile("AbstractSpecs.cs", x =>
            {
                var type = Classes.FirstOrDefault()?.AsTypeInfo();

                Assert.That(type, Is.Not.Null);
                Assert.That(type.IsContext(), Is.False);
            });
        }

        [Test]
        public void BehaviorClassIsNotAContext()
        {
            WithPsiFile("SingleBehavior.cs", x =>
            {
                var type = Classes.FirstOrDefault()?.AsTypeInfo();

                Assert.That(type, Is.Not.Null);
                Assert.That(type.IsContext(), Is.False);
            });
        }

        [Test]
        public void ConcreteClassWithAbstractSpecsIsNotAContext()
        {
            WithPsiFile("ConcreteAndAbstractSpecs.cs", x =>
            {
                var type = Classes.FirstOrDefault()?.AsTypeInfo();

                Assert.That(type, Is.Not.Null);
                Assert.That(type.IsContext(), Is.False);
            });
        }

        [Test]
        public void ClassWithOneSpecIsAContext()
        {
            WithPsiFile("SingleSpec.cs", x =>
            {
                var type = Classes.FirstOrDefault()?.AsTypeInfo();

                Assert.That(type, Is.Not.Null);
                Assert.That(type.IsContext(), Is.True);
            });
        }

        [Test]
        public void ClassWithOneBehaviorSpecIsAContext()
        {
            WithPsiFile("SingleBehaviorSpec.cs", x =>
            {
                var type = Classes.FirstOrDefault(y => y.ShortName == "Specs")?
                    .AsTypeInfo();

                Assert.That(type, Is.Not.Null);
                Assert.That(type.IsContext(), Is.True);
            });
        }
    }
}
