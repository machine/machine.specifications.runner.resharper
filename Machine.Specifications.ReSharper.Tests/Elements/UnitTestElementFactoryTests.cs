using System.Collections.Generic;
using System.Linq;
using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ReSharper.UnitTestFramework;
using Machine.Specifications.ReSharperProvider;
using NSubstitute;
using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests.Elements
{
    [TestFixture]
    public class UnitTestElementFactoryTests : WithSingleProject
    {
        [Test]
        public void CanCreateContext()
        {
            With(() =>
            {
                var factory = new UnitTestElementFactory(ServiceProvider, Project, TargetFrameworkId.Default);

                var element = factory.GetOrCreateContext(new ClrTypeName("MyClass"), BaseTestDataPath, "subject",
                    new[] {"tag1"}, false);

                Assert.That(element, Is.Not.Null);
                Assert.That(element.GetPresentation(), Is.EqualTo("subject, MyClass"));
                Assert.That(element.OwnCategories.Any(x => x.Name == "tag1"), Is.True);
            });
        }

        [Test]
        public void GetsExistingContextFromElementManager()
        {
            With(() =>
            {
                var factory = new UnitTestElementFactory(ServiceProvider, Project, TargetFrameworkId.Default);

                var element1 = factory.GetOrCreateContext(new ClrTypeName("MyClass"), BaseTestDataPath, "subject",
                    new[] { "tag1" }, false);

                ServiceProvider.ElementManager.AddElements(new HashSet<IUnitTestElement> {element1});

                var element2 = factory.GetOrCreateContext(new ClrTypeName("MyClass"), BaseTestDataPath, "subject",
                    new[] { "tag1" }, false);

                Assert.That(element1, Is.Not.Null);
                Assert.That(element2, Is.Not.Null);
                Assert.That(element1, Is.SameAs(element2));
            });
        }

        [Test]
        public void CanCreateContextSpec()
        {
            With(() =>
            {
                var factory = new UnitTestElementFactory(ServiceProvider, Project, TargetFrameworkId.Default);

                var parent = Substitute.For<IUnitTestElement>();
                var element = factory.GetOrCreateContextSpecification(parent, new ClrTypeName("MyClass"), "my_field", false);

                Assert.That(element, Is.Not.Null);
                Assert.That(element.GetPresentation(), Is.EqualTo("my field"));
            });
        }

        [Test]
        public void GetsExistingContextSpecFromElementManager()
        {
            With(() =>
            {
                var factory = new UnitTestElementFactory(ServiceProvider, Project, TargetFrameworkId.Default);

                var parent = factory.GetOrCreateContext(new ClrTypeName("Parent"), BaseTestDataPath, "subject", new string[0], false);
                var element1 = factory.GetOrCreateContextSpecification(parent, new ClrTypeName("MyClass"), "my_field", false);

                ServiceProvider.ElementManager.AddElements(new HashSet<IUnitTestElement> {element1});

                var element2 = factory.GetOrCreateContextSpecification(parent, new ClrTypeName("MyClass"), "my_field", false);

                Assert.That(element1, Is.Not.Null);
                Assert.That(element2, Is.Not.Null);
                Assert.That(element1, Is.SameAs(element2));
            });
        }

        [Test]
        public void CanCreateBehavior()
        {
            With(() =>
            {
                var factory = new UnitTestElementFactory(ServiceProvider, Project, TargetFrameworkId.Default);

                var parent = Substitute.For<IUnitTestElement>();
                var element = factory.GetOrCreateBehavior(parent, new ClrTypeName("MyClass"), "my_field", "FieldType", false);

                Assert.That(element, Is.Not.Null);
                Assert.That(element.GetPresentation(), Is.EqualTo("behaves like my field"));
            });
        }

        [Test]
        public void GetsExistingBehaviorFromElementManager()
        {
            With(() =>
            {
                var factory = new UnitTestElementFactory(ServiceProvider, Project, TargetFrameworkId.Default);

                var parent = factory.GetOrCreateContext(new ClrTypeName("Parent"), BaseTestDataPath, "subject", new string[0], false);
                var element1 = factory.GetOrCreateBehavior(parent, new ClrTypeName("MyClass"), "my_field", "FieldType", false);

                ServiceProvider.ElementManager.AddElements(new HashSet<IUnitTestElement> { element1 });

                var element2 = factory.GetOrCreateBehavior(parent, new ClrTypeName("MyClass"), "my_field", "FieldType", false);

                Assert.That(element1, Is.Not.Null);
                Assert.That(element2, Is.Not.Null);
                Assert.That(element1, Is.SameAs(element2));
            });
        }

        [Test]
        public void CanCreateBehaviorSpec()
        {
            With(() =>
            {
                var factory = new UnitTestElementFactory(ServiceProvider, Project, TargetFrameworkId.Default);

                var parent = factory.GetOrCreateContext(new ClrTypeName("Parent"), BaseTestDataPath, "subject", new string[0], false);
                var element = factory.GetOrCreateBehaviorSpecification(parent, new ClrTypeName("MyClass"), "my_field", false);

                Assert.That(element, Is.Not.Null);
                Assert.That(element.GetPresentation(), Is.EqualTo("my field"));
            });
        }

        [Test]
        public void GetsExistingBehaviorSpecFromElementManager()
        {
            With(() =>
            {
                var factory = new UnitTestElementFactory(ServiceProvider, Project, TargetFrameworkId.Default);

                var context = factory.GetOrCreateContext(new ClrTypeName("Parent"), BaseTestDataPath, "subject", new string[0], false);
                var behavior = factory.GetOrCreateBehavior(context, new ClrTypeName("MyClass"), "my_field", "FieldType", false);
                var element1 = factory.GetOrCreateBehaviorSpecification(behavior, new ClrTypeName("MyClass"), "my_field", false);

                ServiceProvider.ElementManager.AddElements(new HashSet<IUnitTestElement> { element1 });

                var element2 = factory.GetOrCreateBehaviorSpecification(behavior, new ClrTypeName("MyClass"), "my_field", false);

                Assert.That(element1, Is.Not.Null);
                Assert.That(element2, Is.Not.Null);
                Assert.That(element1, Is.SameAs(element2));
            });
        }
    }
}