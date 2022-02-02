using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.ReSharper.Adapters;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.ReSharper.Tasks;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Tests.Adapters
{
    [TestFixture]
    public class RemoteTaskDepotTests
    {
        [Test]
        public void CanGetTaskByElement()
        {
            var depot = new RemoteTaskDepot(GetTasks().ToArray());

            foreach (var element in GetElements())
            {
                Assert.NotNull(depot[element]);
            }
        }

        private IEnumerable<RemoteTask> GetTasks()
        {
            yield return MspecContextRemoteTask.ToServer("Namespace.Context", null, null, null);
            yield return MspecSpecificationRemoteTask.ToServer("Namespace.Context", "should", null, null, null, null);
            yield return MspecBehaviorSpecificationRemoteTask.ToServer("Namespace.Context.behaves_like", "Namespace.Context", "behaves_like", null);
            yield return MspecSpecificationRemoteTask.ToServer("Namespace.Context", "should_behave", "Namespace.ABehavior", null, null, null);
            yield return MspecSpecificationRemoteTask.ToServer("Namespace.Context", "should_not_behave", "Namespace.ABehavior", null, null, null);
        }

        private IEnumerable<IMspecElement> GetElements()
        {
            var context = new ContextElement("Namespace.Context", string.Empty);
            var behavior = new BehaviorElement(context, "Namespace.ABehavior", "behaves_like");
            var specification1 = new SpecificationElement(context, "should");
            var specification2 = new SpecificationElement(context, "should_behave", behavior);
            var specification3 = new SpecificationElement(context, "should_not_behave", behavior);

            return new IMspecElement[]
            {
                context,
                behavior,
                specification1,
                specification2,
                specification3
            };
        }
    }
}
