using NUnit.Framework;

namespace Machine.Specifications.ReSharper.Tests.Metadata
{
    [TestFixture]
    public class ContextTests : MetadataTests
    {
        [Test]
        public void SingleSpecIsValid()
        {
            ExecuteTest("SingleSpec.cs");
        }

        [Test]
        public void AbstractSpecsIsIgnored()
        {
            ExecuteTest("AbstractSpecs.cs");
        }
    }
}