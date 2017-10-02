using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests.Explorers
{
    [TestFixture]
    public class BehaviorTests : ReflectionWithSingleProject
    {
        [Test]
        public void LoneBehaviorIsIgnored()
        {
            WithFile("SingleBehavior.cs", observer =>
            {
                Assert.That(observer.Elements, Is.Empty);
            });
        }
    }
}
