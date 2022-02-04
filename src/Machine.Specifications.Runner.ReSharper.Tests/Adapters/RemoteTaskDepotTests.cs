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
        public void CanGetContextByElement()
        {
            var contextTask = MspecContextRemoteTask.ToServer("Namespace.Context", null, null, null);
            var contextElement = new ContextElement("Namespace.Context", string.Empty);

            var depot = new RemoteTaskDepot(new RemoteTask[] { contextTask });

            Assert.NotNull(depot[contextElement]);
        }

        [Test]
        public void CanGetSpecificationByElement()
        {
            var contextTask = MspecContextRemoteTask.ToServer("Namespace.Context", null, null, null);
            var specificationTask = MspecSpecificationRemoteTask.ToServer("Namespace.Context", "should", null, null, null, null);

            var contextElement = new ContextElement("Namespace.Context", string.Empty);
            var specificationElement = new SpecificationElement(contextElement, "should");

            var depot = new RemoteTaskDepot(new RemoteTask[] { contextTask, specificationTask });

            Assert.NotNull(depot[specificationElement]);
        }

        [Test]
        public void CanGetBehaviorByElement()
        {
            var contextTask = MspecContextRemoteTask.ToServer("Namespace.Context", null, null, null);
            var behaviorTask = MspecSpecificationRemoteTask.ToServer("Namespace.Context", "behaves_like", "Namespace.ABehavior", null, null, null);

            var contextElement = new ContextElement("Namespace.Context", string.Empty);
            var behaviorElement = new BehaviorElement(contextElement, "Namespace.ABehavior", "behaves_like");

            var depot = new RemoteTaskDepot(new RemoteTask[] { contextTask, behaviorTask });

            Assert.NotNull(depot[behaviorElement]);
        }

        [Test]
        public void CanGetBehaviorSpecificationByElement()
        {
            var contextTask = MspecContextRemoteTask.ToServer("Namespace.Context", null, null, null);
            var behaviorTask = MspecSpecificationRemoteTask.ToServer("Namespace.Context", "behaves_like", "Namespace.ABehavior", null, null, null);
            var specificationTask = MspecBehaviorSpecificationRemoteTask.ToServer("Namespace.Context.behaves_like", "Namespace.Context", "should_behave", null);

            var contextElement = new ContextElement("Namespace.Context", string.Empty);
            var specificationElement = new SpecificationElement(contextElement, "should");
            var behaviorElement = new BehaviorElement(contextElement, "Namespace.ABehavior", "behaves_like");
            var behaviorSpecificationElement = new SpecificationElement(contextElement, "should_behave", behaviorElement);

            var depot = new RemoteTaskDepot(new RemoteTask[] { contextTask, behaviorTask, specificationTask });

            Assert.NotNull(depot[specificationElement]);
        }

        [Test]
        public void CanGetBehaviorWhenThereIsSpecificationWithSameName()
        {
            var contextTask = MspecContextRemoteTask.ToServer("Namespace.Context", null, null, null);
            var specificationTask = MspecSpecificationRemoteTask.ToServer("Namespace.Context", "should", null, null, null, null);
            var behaviorTask = MspecSpecificationRemoteTask.ToServer("Namespace.Context", "behaves_like", "Namespace.ABehavior", null, null, null);
            var behaviorSpecificationTask = MspecBehaviorSpecificationRemoteTask.ToServer("Namespace.Context.behaves_like", "Namespace.Context", "should_behave", null);

            var contextElement = new ContextElement("Namespace.Context", string.Empty);
            var behaviorElement = new BehaviorElement(contextElement, "Namespace.ABehavior", "behaves_like");
            var specificationElement = new SpecificationElement(contextElement, "should_behave", behaviorElement);

            var depot = new RemoteTaskDepot(new RemoteTask[] { contextTask, behaviorTask, specificationTask });

            Assert.NotNull(depot[specificationElement]);
        }

        private IEnumerable<RemoteTask> GetTasks()
        {
            yield return MspecContextRemoteTask.ToServer("Namespace.Context", null, null, null);
            yield return MspecSpecificationRemoteTask.ToServer("Namespace.Context", "should", null, null, null, null);
            yield return MspecSpecificationRemoteTask.ToServer("Namespace.Context", "behaves_like", "Namespace.ABehavior", null, null, null);
            yield return MspecBehaviorSpecificationRemoteTask.ToServer("Namespace.Context.behaves_like", "Namespace.Context", "should_behave", null);
            yield return MspecBehaviorSpecificationRemoteTask.ToServer("Namespace.Context.behaves_like", "Namespace.Context", "should_not_behave", null);
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
