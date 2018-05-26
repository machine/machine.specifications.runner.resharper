using System.Linq;
using Machine.Specifications.ReSharperProvider;
using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests.Psi
{
    [TestFixture]
    public class IgnoreTests : PsiTests
    {
        [Test]
        public void NoAttributeIsntIgnored()
        {
            //WithPsiFile("SingleSpec.cs", x => Assert.That(Fields.All(y => y.IsIgnored()), Is.False));
        }

        [Test]
        public void FieldIsIgnored()
        {
            WithPsiFile("IgnoredField.cs", x => Assert.That(Field().IsIgnored(), Is.True));
        }

        [Test]
        public void ClassIsIgnored()
        {
            WithPsiFile("IgnoredClass.cs", x => Assert.That(Type().IsIgnored(), Is.True));
        }

        [Test]
        public void FieldWithIgnoredClassIsIgnored()
        {
            // TODO: Test that field inside an ignored class is ignored
        }
    }
}
