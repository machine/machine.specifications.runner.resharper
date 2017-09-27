using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Reader.Impl;
using Machine.Specifications.ReSharperProvider.Elements;
using NSubstitute;
using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests.Elements
{
    [TestFixture]
    public class ContextElementTests : WithSingleProject
    {
        [Test]
        public void CanGetHashCode()
        {
            With(() =>
            {
                var id = CreateId("id");

                var element = new ContextElement(id, Substitute.For<IClrTypeName>(), ServiceProvider, "subject", false);

                Assert.That(element.GetHashCode(), Is.Not.EqualTo(0));
            });
        }

        [Test]
        public void ContextsWithSamePropertiesAreEqual()
        {
            With(() =>
            {
                var id = CreateId("id");
                var type = Substitute.For<IClrTypeName>();

                var element1 = new ContextElement(id, type, ServiceProvider, "subject", false);
                var element2 = new ContextElement(id, type, ServiceProvider, "subject", false);

                Assert.That(element1, Is.EqualTo(element2));
            });
        }

        [Test]
        public void ContextPresentationIsValid()
        {
            With(() =>
            {
                var id = CreateId("id");
                var type = new ClrTypeName("Namespace.Elements.my_class_name");

                var element = new ContextElement(id, type, ServiceProvider, "my subject", false);

                Assert.That(element.GetPresentation(null, false), Is.EqualTo("my subject, my class name"));
            });
        }

        [Test]
        public void ContextWithNoSubjectPresentationIsValid()
        {
            With(() =>
            {
                var id = CreateId("id");
                var type = new ClrTypeName("Namespace.Elements.my_class_name");

                var element = new ContextElement(id, type, ServiceProvider, null, false);

                Assert.That(element.GetPresentation(null, false), Is.EqualTo("my class name"));
            });
        }
    }
}
