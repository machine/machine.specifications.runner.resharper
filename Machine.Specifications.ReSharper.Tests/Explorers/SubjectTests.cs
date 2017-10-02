using System.Linq;
using Machine.Specifications.ReSharperProvider.Elements;
using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests.Explorers
{
    [TestFixture]
    public class SubjectTests : ReflectionWithSingleProject
    {
        [Test]
        public void CanProcessInheritedSubjects()
        {
            WithFile("InheritedSubjects.cs", observer =>
            {
                var context = observer.Elements
                    .OfType<ContextElement>()
                    .FirstOrDefault();

                var spec = observer.Elements
                    .OfType<ContextSpecificationElement>()
                    .FirstOrDefault();

                Assert.That(observer.Elements.Count, Is.EqualTo(2));
                Assert.That(context?.GetPresentation(context, false), Is.EqualTo("specifications, Spec"));
                Assert.That(spec?.GetPresentation(context, false), Is.EqualTo("is something"));
            });
        }
    }
}
