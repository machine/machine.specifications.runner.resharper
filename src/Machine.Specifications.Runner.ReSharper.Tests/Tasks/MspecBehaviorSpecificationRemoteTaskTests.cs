using Machine.Specifications.Runner.ReSharper.Tasks;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests.Tasks
{
    [TestFixture]
    public class MspecBehaviorSpecificationRemoteTaskTests
    {
        [Test]
        public void ServerTaskHasCorrectId()
        {
            var task = MspecBehaviorSpecificationRemoteTask.ToServer("Namespace.Context.behaves_like", "Namespace.Context", "should", null);

            Assert.AreEqual("Namespace.Context.behaves_like.should", task.TestId);
        }
    }
}
