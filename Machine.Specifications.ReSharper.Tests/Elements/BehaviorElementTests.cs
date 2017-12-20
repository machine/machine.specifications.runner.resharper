using JetBrains.Metadata.Reader.API;
using Machine.Specifications.ReSharperProvider.Elements;
using NSubstitute;
using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests.Elements
{
    [TestFixture]
    public class BehaviorElementTests : ReflectionWithSingleProject
    {
        [Test]
        public void CanGetHashCode()
        {
            With(() =>
            {
                var id = CreateId("id");

                var element = new BehaviorElement(id, CreateUnitTestElement(),
                    Substitute.For<IClrTypeName>(), ServiceProvider, "field", false);

                Assert.That(element.GetHashCode(), Is.Not.EqualTo(0));
            });
        }

        [Test]
        public void BehaviorsWithSamePropertiesAreEqual()
        {
            With(() =>
            {
                var id = CreateId("id");
                var type = Substitute.For<IClrTypeName>();

                var element1 = new BehaviorElement(id, CreateUnitTestElement(),
                    type, ServiceProvider, "field", false);

                var element2 = new BehaviorElement(id, CreateUnitTestElement(),
                    type, ServiceProvider, "field", false);

                Assert.That(element1, Is.EqualTo(element2));
            });
        }

        [Test]
        public void BehaviorPresentationIsValid()
        {
            With(() =>
            {
                var id = CreateId("id");

                var element = new BehaviorElement(id, CreateUnitTestElement(),
                    Substitute.For<IClrTypeName>(), ServiceProvider, "field_is_something", false);

                Assert.That(element.GetPresentation(null, false), Is.EqualTo("behaves like field is something"));
            });
        }
    }
}
