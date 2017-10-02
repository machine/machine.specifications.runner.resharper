using System.Linq;
using Machine.Specifications.ReSharperProvider.Elements;
using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests.Explorers
{
    [TestFixture]
    public class SpecificationTests : ReflectionWithSingleProject
    {
        [Test]
        public void CanProcessSingleSpecification()
        {
            WithFile("SingleSpec.cs", observer =>
            {
                var context = observer.Elements
                    .OfType<ContextElement>()
                    .FirstOrDefault();

                var spec = observer.Elements
                    .OfType<ContextSpecificationElement>()
                    .FirstOrDefault();

                Assert.That(observer.Elements.Count, Is.EqualTo(2));
                
                Assert.That(context?.GetPresentation(context, false), Is.EqualTo("Simple, Simple"));
                Assert.That(context?.TypeName.FullName, Is.EqualTo("Data.Simple"));
                Assert.That(context?.Explicit, Is.False);
                Assert.That(context?.OwnCategories, Is.Empty);

                Assert.That(spec?.GetPresentation(context, false), Is.EqualTo("is true"));
                Assert.That(spec?.ShortName, Is.EqualTo("is_true"));
                Assert.That(spec?.Explicit, Is.False);
                Assert.That(spec?.OwnCategories, Is.Empty);
            });
        }
    }
}