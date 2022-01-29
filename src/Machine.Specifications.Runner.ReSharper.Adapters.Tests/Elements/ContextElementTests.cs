using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Tests.Elements
{
    [TestFixture]
    public class ContextElementTests
    {
        [Test]
        public void SetsCorrectId()
        {
            var context = new ContextElement("Namespace.ContextType", "subject");

            Assert.That(context.Id, Is.EqualTo("Namespace.ContextType"));
        }
    }
}
