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
        public void CanProcessSimple()
        {
            WithPsiFile("SingleSpec.cs", (file, project) =>
            {
                var observer = new TestUnitTestElementObserver();

                ProcessPsiFile(file, project, observer);

                Assert.That(observer.NewElements.Count, Is.EqualTo(2));
                Assert.That(observer.NewElements.OfType<ContextElement>().Count(), Is.EqualTo(1));
                Assert.That(observer.NewElements.OfType<ContextSpecificationElement>().Count(), Is.EqualTo(1));
            });
        }

        private void ProcessPsiFile(IFile file, IProject project, IUnitTestElementsObserver observer)
        {
            var serviceProvider = Solution.GetComponent<MspecServiceProvider>();
            var factory = new UnitTestElementFactory(serviceProvider, project, observer.TargetFrameworkId);

            var explorer = new MspecPsiFileExplorer(factory, file, observer, () => false);
            file.ProcessDescendants(explorer);
        }
    }
}