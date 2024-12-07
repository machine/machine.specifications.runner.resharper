using Machine.Specifications.Runner.ReSharper.Tasks;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests.Tasks;

[TestFixture]
public class MspecSpecificationRemoteTaskTests
{
    [Test]
    public void SpecificationServerTaskHasCorrectId()
    {
        var task = MspecSpecificationRemoteTask.ToServer("Namespace.Context", "should", null, null, null, null);

        Assert.That(task.TestId, Is.EqualTo("Namespace.Context.should"));
    }

    [Test]
    public void BehaviorServerTaskHasCorrectId()
    {
        var task = MspecSpecificationRemoteTask.ToServer("Namespace.Context", "behaves_like", "Namespace.ABehavior", null, null, null);

        Assert.That(task.TestId, Is.EqualTo("Namespace.Context.behaves_like"));
    }
}
