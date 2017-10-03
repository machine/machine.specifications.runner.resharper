using System.Linq;
using Machine.Specifications.ReSharperProvider.Elements;
using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests.Explorers
{
    [TestFixture]
    public class IgnoreTests : ReflectionWithSingleProject
    {
        [Test]
        public void IgnoredContextAlsoIgnoresSpecs()
        {
            WithFile("IgnoredClass.cs", observer =>
            {
                var context = observer.Elements
                    .OfType<ContextElement>()
                    .FirstOrDefault();

                var spec = observer.Elements
                    .OfType<ContextSpecificationElement>()
                    .FirstOrDefault();

                Assert.That(observer.Elements.Count, Is.EqualTo(2));

                Assert.That(context?.Explicit, Is.True);
                Assert.That(context?.ExplicitReason, Is.EqualTo("Ignored"));

                Assert.That(spec?.Explicit, Is.True);
                Assert.That(spec?.ExplicitReason, Is.EqualTo("Ignored"));
            });
        }
    }
}
