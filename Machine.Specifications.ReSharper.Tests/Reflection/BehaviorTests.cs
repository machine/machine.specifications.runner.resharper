using Machine.Specifications.ReSharperProvider;
using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests.Reflection
{
    [TestFixture]
    public class BehaviorTests : CombinedProjectTest
    {
        [Test]
        public void SingleBehaviorSpecIsValid()
        {
            WithFile("SingleBehavior.cs", x => Assert.That(x.Type().IsBehaviorContainer(), Is.True));
        }

        [Test]
        public void BehaviorWithNoSpecsIsNotValid()
        {
            WithFile("BehaviorNoSpecs.cs", x => Assert.That(x.Type().IsBehaviorContainer(), Is.False));
        }

        [Test]
        public void AbstractBehaviorWithSpecsIsNotValid()
        {
            WithFile("BehaviorAbstract.cs", x => Assert.That(x.Type().IsBehaviorContainer(), Is.False));
        }
    }
}
