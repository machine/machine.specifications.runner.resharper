using JetBrains.Metadata.Reader.Impl;
using Machine.Specifications.Runner.ReSharper.Elements;
using Machine.Specifications.Runner.ReSharper.Tests.TestFramework;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests.Elements;

[TestFixture]
public class MspecContextTestElementTests : UnitTestElementTestBase
{
    [Test]
    public void CreatesValidId()
    {
        WithDiscovery(() =>
        {
            var element = new MspecContextTestElement(new ClrTypeName("Namespace.Context"), null, null);

            Assert.AreEqual("Namespace.Context", element.NaturalId.TestId);
        });
    }
}
