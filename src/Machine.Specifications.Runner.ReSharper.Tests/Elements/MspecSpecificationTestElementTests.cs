using JetBrains.Metadata.Reader.Impl;
using Machine.Specifications.Runner.ReSharper.Elements;
using Machine.Specifications.Runner.ReSharper.Tests.TestFramework;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests.Elements
{
    [TestFixture]
    public class MspecSpecificationTestElementTests : UnitTestElementTestBase
    {
        [Test]
        public void CreatesValidId()
        {
            WithDiscovery(() =>
            {
                var context = new MspecContextTestElement(new ClrTypeName("Namespace.Context"), null, null);
                var specification = new MspecSpecificationTestElement(context, "should", null, null, null);

                Assert.AreEqual("Namespace.Context.should", specification.NaturalId.TestId);
            });
        }
    }
}
