using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.UnitTestFramework;
using Machine.Specifications.ReSharperProvider.Elements;
using NSubstitute;
using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests.Elements
{
    [TestFixture]
    public class ContextSpecificationElementTests : WithSingleProject
    {
        [Test]
        public void CanGetHashCode()
        {
            With(() =>
            {
                var id = CreateId("id");

                var element = new ContextSpecificationElement(id, Substitute.For<IUnitTestElement>(),
                    Substitute.For<IClrTypeName>(), ServiceProvider, "field", false);

                Assert.That(element.GetHashCode(), Is.Not.EqualTo(0));
            });
        }

        [Test]
        public void ContextSpecsWithSamePropertiesAreEqual()
        {
            With(() =>
            {
                var id = CreateId("id");
                var type = Substitute.For<IClrTypeName>();

                var element1 = new ContextSpecificationElement(id, Substitute.For<IUnitTestElement>(),
                    type, ServiceProvider, "field", false);

                var element2 = new ContextSpecificationElement(id, Substitute.For<IUnitTestElement>(),
                    type, ServiceProvider, "field", false);

                Assert.That(element1, Is.EqualTo(element2));
            });
        }

        [Test]
        public void ContextSpecPresentationIsValid()
        {
            With(() =>
            {
                var id = CreateId("id");

                var element = new ContextSpecificationElement(id, Substitute.For<IUnitTestElement>(),
                    Substitute.For<IClrTypeName>(), ServiceProvider, "field_is_something", false);

                Assert.That(element.GetPresentation(null, false), Is.EqualTo("field is something"));
            });
        }
    }
}
