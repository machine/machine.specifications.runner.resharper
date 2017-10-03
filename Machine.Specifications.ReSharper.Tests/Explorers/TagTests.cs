using System.Linq;
using Machine.Specifications.ReSharperProvider.Elements;
using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests.Explorers
{
    [TestFixture]
    public class TagTests : ReflectionWithSingleProject
    {
        [Test]
        public void SpecsTagsAreSameAsContext()
        {
            WithFile("SingleTag.cs", observer =>
            {
                var context = observer.Elements
                    .OfType<ContextElement>()
                    .FirstOrDefault();

                var spec = observer.Elements
                    .OfType<ContextSpecificationElement>()
                    .FirstOrDefault();

                Assert.That(observer.Elements.Count, Is.EqualTo(2));

                Assert.That(context?.GetPresentation(context, false), Is.EqualTo("Specs"));
                Assert.That(context?.OwnCategories.Count, Is.EqualTo(1));
                Assert.That(context?.OwnCategories.FirstOrDefault()?.Name, Is.EqualTo("Taggy"));

                Assert.That(spec?.GetPresentation(context, false), Is.EqualTo("is something"));
                Assert.That(spec?.OwnCategories.FirstOrDefault()?.Name, Is.EqualTo("Taggy"));
            });
        }
    }
}
