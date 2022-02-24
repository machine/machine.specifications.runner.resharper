using JetBrains.Metadata.Reader.Impl;
using Machine.Specifications.Runner.ReSharper.Elements;
using Machine.Specifications.Runner.ReSharper.Tests.TestFramework;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests.Elements
{
    [TestFixture]
    public class MspecBehaviorSpecificationTestElementTests : UnitTestElementTestBase
    {
        [Test]
        public void CreatesValidId()
        {
            WithDiscovery(() =>
            {
                var context = new MspecContextTestElement(new ClrTypeName("Namespace.Context"), null, null);
                var specification = new MspecSpecificationTestElement(context, "behaves_like", "Namespace.ABehavior", null, null);
                var behaviorSpecification = new MspecBehaviorSpecificationTestElement(specification, "should", null);

                Assert.AreEqual("Namespace.Context.behaves_like.should", behaviorSpecification.NaturalId.TestId);
            });
        }
    }
}
