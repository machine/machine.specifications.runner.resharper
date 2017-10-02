using System.Linq;
using Machine.Specifications.ReSharperProvider.Elements;
using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests.Explorers
{
    [TestFixture]
    public class BehaviorSpecificationTests : ReflectionWithSingleProject
    {
        [Test]
        public void BehaviorSpecIsParsed()
        {
            WithFile("SingleBehaviorSpec.cs", observer =>
            {
                var context = observer.Elements
                    .OfType<ContextElement>()
                    .FirstOrDefault();

                var behavior = observer.Elements
                    .OfType<BehaviorElement>()
                    .FirstOrDefault();

                var spec = observer.Elements
                    .OfType<BehaviorSpecificationElement>()
                    .FirstOrDefault();

                Assert.That(observer.Elements.Count, Is.EqualTo(3));

                Assert.That(behavior?.Context, Is.SameAs(context));
                Assert.That(behavior?.ShortName, Is.EqualTo("a_class"));
                Assert.That(behavior?.FieldType, Is.EqualTo("Data.TestBehavior"));
                Assert.That(behavior?.GetPresentation(behavior, false), Is.EqualTo("behaves like a class"));

                Assert.That(spec?.Behavior, Is.SameAs(behavior));
                Assert.That(spec?.ShortName, Is.EqualTo("is_something"));
                Assert.That(spec?.GetPresentation(spec, false), Is.EqualTo("is something"));
            });
        }
    }
}