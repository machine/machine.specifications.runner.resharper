using Machine.Specifications.ReSharperProvider;
using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests.Psi
{
    [TestFixture]
    public class BehaviorTests : PsiTests
    {
        [Test]
        public void SingleBehaviorSpecIsValid()
        {
            WithPsiFile("SingleBehavior.cs", x => Assert.That(Type().IsBehaviorContainer(), Is.True));
        }

        [Test]
        public void BehaviorWithNoSpecsIsNotValid()
        {
            WithPsiFile("BehaviorNoSpecs.cs", x => Assert.That(Type().IsBehaviorContainer(), Is.False));
        }

        [Test]
        public void AbstractBehaviorWithSpecsIsNotValid()
        {
            WithPsiFile("BehaviorAbstract.cs", x => Assert.That(Type().IsBehaviorContainer(), Is.False));
        }
    }
}
