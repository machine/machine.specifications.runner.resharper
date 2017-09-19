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
    }
}
