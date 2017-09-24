using Machine.Specifications.ReSharperProvider;
using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests.Reflection
{
    [TestFixture]
    public class SubjectTests : PsiTests
    {
        [Test]
        public void NoSubjectReturnsEmptyString()
        {
            WithFile("EmptyClass.cs", x => Assert.That(Type().GetSubject(), Is.EqualTo(string.Empty)));
        }

        [Test]
        public void GetsSubjectTypeAsString()
        {
            WithFile("SingleSpec.cs", x => Assert.That(Type().GetSubject(), Is.EqualTo("Simple")));
        }

        [Test]
        public void SubjectIsRetrievedFromBaseClass()
        {
            WithFile("BaseClassWithSubject.cs", x => Assert.That(Type("Spec").GetSubject(), Is.EqualTo("BaseClass")));
        }

        [Test]
        public void NearestTypeSubjectIsUsed()
        {
            WithFile("InheritedSubjects.cs", x => Assert.That(Type("Spec").GetSubject(), Is.EqualTo("specifications")));
        }

        [Test]
        public void CompositeSubjectIsUsed()
        {
            WithFile("CompositeSubject.cs", x => Assert.That(Type().GetSubject(), Is.EqualTo("Specs plus plus")));
        }

        [Test]
        public void NestedClassSubjectIsUsed()
        {
            WithFile("NestedSubject.cs", x => Assert.That(Type("InnerSpecs").GetSubject(), Is.EqualTo("InnerSpecs plus")));
        }

        [Test]
        public void ParentClassSubjectIsUsed()
        {
            WithFile("ParentSubject.cs", x => Assert.That(Type("InnerSpecs").GetSubject(), Is.EqualTo("Specs")));
        }

        [Test]
        public void ParentBaseClassSubjectIsUsed()
        {
            WithFile("ParentBaseSubject.cs", x => Assert.That(Type("InnerSpecs").GetSubject(), Is.EqualTo("BaseClass")));
        }
    }
}
