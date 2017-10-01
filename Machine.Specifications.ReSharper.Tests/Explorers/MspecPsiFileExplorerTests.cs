using System.Linq;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.FeaturesTestFramework.UnitTesting;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
using Machine.Specifications.ReSharperProvider;
using Machine.Specifications.ReSharperProvider.Elements;
using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests.Explorers
{
    [TestFixture]
    public class MspecPsiFileExplorerTests : ReflectionWithSingleProject
    {
        [Test]
        public void CanProcessSingleSpecification()
        {
            WithPsiFile("SingleSpec.cs", (file, project) =>
            {
                var observer = ProcessPsiFile(file, project);

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

        [Test]
        public void SpecsTagsAreSameAsContext()
        {
            WithPsiFile("SingleTag.cs", (file, project) =>
            {
                var observer = ProcessPsiFile(file, project);

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

        [Test]
        public void IgnoredContextAlsoIgnoresSpecs()
        {
            WithPsiFile("IgnoredClass.cs", (file, project) =>
            {
                var observer = ProcessPsiFile(file, project);

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

        [Test]
        public void CanProcessInheritedSubjects()
        {
            WithPsiFile("InheritedSubjects.cs", (file, project) =>
            {
                var observer = ProcessPsiFile(file, project);

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

        private TestUnitTestElementObserver ProcessPsiFile(IFile file, IProject project)
        {
            var serviceProvider = Solution.GetComponent<MspecServiceProvider>();
            var observer = new TestUnitTestElementObserver();
            var factory = new UnitTestElementFactory(serviceProvider, project, observer.TargetFrameworkId);

            var explorer = new MspecPsiFileExplorer(factory, file, observer, () => false);
            file.ProcessDescendants(explorer);

            return observer;
        }
    }
}