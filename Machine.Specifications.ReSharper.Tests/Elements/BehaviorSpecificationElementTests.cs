using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.UnitTestFramework;
using Machine.Specifications.ReSharperProvider.Presentation;
using NSubstitute;
using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests.Elements
{
    [TestFixture]
    public class BehaviorSpecificationElementTests : WithSingleProject
    {
        [Test]
        public void CanGetHashCode()
        {
            With(() =>
            {
                var id = CreateId("id");

                var element = new BehaviorSpecificationElement(id, Substitute.For<IUnitTestElement>(),
                    Substitute.For<IClrTypeName>(), ServiceProvider, "field", false);

                Assert.That(element.GetHashCode(), Is.Not.EqualTo(0));
            });
        }

        [Test]
        public void BehaviorSpecsWithSamePropertiesAreEqual()
        {
            With(() =>
            {
                var id = CreateId("id");
                var type = Substitute.For<IClrTypeName>();

                var context = new ContextElement(id, Substitute.For<IClrTypeName>(), ServiceProvider, "subject", false);

                var parent = new BehaviorElement(id, context,
                    Substitute.For<IClrTypeName>(), ServiceProvider, "field", false, "type");

                var element1 = new BehaviorSpecificationElement(id, parent, type, ServiceProvider, "field", false);
                var element2 = new BehaviorSpecificationElement(id, parent, type, ServiceProvider, "field", false);

                Assert.That(element1, Is.EqualTo(element2));
            });
        }

        [Test]
        public void BehaviorSpecPresentationIsValid()
        {
            With(() =>
            {
                var id = CreateId("id");

                var element = new BehaviorSpecificationElement(id, Substitute.For<IUnitTestElement>(),
                    Substitute.For<IClrTypeName>(), ServiceProvider, "field_is_something", false);

                Assert.That(element.GetPresentation(null, false), Is.EqualTo("field is something"));
            });
        }
    }
}
