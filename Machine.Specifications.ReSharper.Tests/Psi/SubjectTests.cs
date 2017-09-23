using Machine.Specifications.ReSharperProvider;
using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests.Psi
{
    [TestFixture]
    public class SubjectTests : PsiTests
    {
        [Test]
        public void NoSubjectReturnsEmptyString()
        {
            WithPsiFile("EmptyClass.cs", x => Assert.That(Type().GetSubject(), Is.EqualTo(string.Empty)));
        }

        [Test]
        public void GetsSubjectTypeAsString()
        {
            WithPsiFile("SingleSpec.cs", x => Assert.That(Type().GetSubject(), Is.EqualTo("Simple")));
        }

        [Test]
        public void SubjectIsRetrievedFromBaseClass()
        {
            WithPsiFile("BaseClassWithSubject.cs", x => Assert.That(Type("Spec").GetSubject(), Is.EqualTo("BaseClass")));
        }

        [Test]
        public void NearestTypeSubjectIsUsed()
        {
            WithPsiFile("InheritedSubjects.cs", x => Assert.That(Type("Spec").GetSubject(), Is.EqualTo("specifications")));
        }

        [Test]
        public void CompositeSubjectIsUsed()
        {
            WithPsiFile("CompositeSubject.cs", x => Assert.That(Type().GetSubject(), Is.EqualTo("Specs plus plus")));
        }

        [Test]
        public void NestedClassSubjectIsUsed()
        {
            WithPsiFile("NestedSubject.cs", x => Assert.That(Type("InnerSpecs").GetSubject(), Is.EqualTo("InnerSpecs plus")));
        }

        [Test]
        public void ParentClassSubjectIsUsed()
        {
            WithPsiFile("ParentSubject.cs", x => Assert.That(Type("InnerSpecs").GetSubject(), Is.EqualTo("Specs")));
        }

        [Test]
        public void ParentBaseClassSubjectIsUsed()
        {
            WithPsiFile("ParentBaseSubject.cs", x => Assert.That(Type("InnerSpecs").GetSubject(), Is.EqualTo("BaseClass")));
        }
    }
}
