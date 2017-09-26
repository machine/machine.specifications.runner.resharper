using System.Linq;
using Machine.Specifications.ReSharperProvider;
using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests.Reflection
{
    [TestFixture]
    public class IgnoreTests : ReflectionWithSingleProject
    {
        [Test]
        public void NoAttributeIsntIgnored()
        {
            WithFile("SingleSpec.cs", x =>
            {
                Assert.That(x.Fields, Is.Not.Empty);
                Assert.That(x.Fields.All(y => y.IsIgnored()), Is.False);
            });
        }

        [Test]
        public void FieldIsIgnored()
        {
            WithFile("IgnoredField.cs", x => Assert.That(x.Field().IsIgnored(), Is.True));
        }

        [Test]
        public void ClassIsIgnored()
        {
            WithFile("IgnoredClass.cs", x => Assert.That(x.Type().IsIgnored(), Is.True));
        }

        [Test]
        public void FieldWithIgnoredClassIsIgnored()
        {
            // TODO: Test that field inside an ignored class is ignored
        }
    }
}
