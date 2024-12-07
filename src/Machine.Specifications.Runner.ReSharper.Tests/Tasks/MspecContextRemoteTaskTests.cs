using Machine.Specifications.Runner.ReSharper.Tasks;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests.Tasks;

[TestFixture]
public class MspecContextRemoteTaskTests
{
    [Test]
    public void ServerTaskHasCorrectId()
    {
        var task = MspecContextRemoteTask.ToServer("Namespace.Context", null, null, null);

        Assert.That(task.TestId, Is.EqualTo("Namespace.Context"));
    }
}
