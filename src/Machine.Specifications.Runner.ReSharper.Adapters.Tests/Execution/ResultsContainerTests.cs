using System.Collections.Generic;
using System.Linq;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.ReSharper.Adapters.Execution;
using Xunit;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Tests.Execution
{
    public class ResultsContainerTests
    {
        [Fact]
        public void CanGetResultsById()
        {
            var elements = GetElements().ToArray();

            var results = new ResultsContainer(elements);

            Assert.NotNull(results.Started<IContextElement>("Namespace.ContextType"));
            Assert.NotNull(results.Started<IBehaviorElement>("Namespace.ContextType.Namespace.ABehavior"));
            Assert.NotNull(results.Started<ISpecificationElement>("Namespace.ContextType.should_be"));
            Assert.NotNull(results.Started<ISpecificationElement>("Namespace.ContextType.Namespace.ABehavior.should_be"));
            Assert.NotNull(results.Started<ISpecificationElement>("Namespace.ContextType.Namespace.ABehavior.should_not_be"));
        }

        [Fact]
        public void CanReserveBehaviorType()
        {
            var elements = GetElements().ToArray();

            var results = new ResultsContainer(elements);

            var behavior1 = results.Started<IBehaviorElement>("Namespace.ContextType.Namespace.ABehavior");
            var behavior2 = results.Started<IBehaviorElement>("Namespace.ContextType.Namespace.ABehavior");

            Assert.NotNull(behavior1);
            Assert.Null(behavior2);
        }

        [Fact]
        public void CanSetAllSuccessful()
        {
            var elements = GetElements().ToArray();

            var results = new ResultsContainer(elements);

            results.Started<IContextElement>("Namespace.ContextType");
            results.Started<IBehaviorElement>("Namespace.ContextType.Namespace.ABehavior");

            var specification1 = results.Started<ISpecificationElement>("Namespace.ContextType.should_be");
            var specification2 = results.Started<ISpecificationElement>("Namespace.ContextType.Namespace.ABehavior.should_be");
            var specification3 = results.Started<ISpecificationElement>("Namespace.ContextType.Namespace.ABehavior.should_not_be");

            results.Finished(specification1!, true);
            results.Finished(specification2!, true);
            results.Finished(specification3!, true);

            var context = results.Get("Namespace.ContextType");
            var behavior = results.Get("Namespace.ContextType.Namespace.ABehavior");

            Assert.True(context.IsSuccessful);
            Assert.True(behavior.IsSuccessful);
        }

        [Fact]
        public void CanSetBehaviorAsFailed()
        {
            var elements = GetElements().ToArray();

            var results = new ResultsContainer(elements);

            results.Started<IContextElement>("Namespace.ContextType");
            results.Started<IBehaviorElement>("Namespace.ContextType.Namespace.ABehavior");

            var specification1 = results.Started<ISpecificationElement>("Namespace.ContextType.should_be");
            var specification2 = results.Started<ISpecificationElement>("Namespace.ContextType.Namespace.ABehavior.should_be");
            var specification3 = results.Started<ISpecificationElement>("Namespace.ContextType.Namespace.ABehavior.should_not_be");

            results.Finished(specification1!, true);
            results.Finished(specification2!, true);
            results.Finished(specification3!, false);

            var context = results.Get("Namespace.ContextType");
            var behavior = results.Get("Namespace.ContextType.Namespace.ABehavior");

            Assert.False(context.IsSuccessful);
            Assert.False(behavior.IsSuccessful);
        }

        [Fact]
        public void CanSetContextAsFailed()
        {
            var elements = GetElements().ToArray();

            var results = new ResultsContainer(elements);

            results.Started<IContextElement>("Namespace.ContextType");
            results.Started<IBehaviorElement>("Namespace.ContextType.Namespace.ABehavior");

            var specification1 = results.Started<ISpecificationElement>("Namespace.ContextType.should_be");
            var specification2 = results.Started<ISpecificationElement>("Namespace.ContextType.Namespace.ABehavior.should_be");
            var specification3 = results.Started<ISpecificationElement>("Namespace.ContextType.Namespace.ABehavior.should_not_be");

            results.Finished(specification1!, false);
            results.Finished(specification2!, true);
            results.Finished(specification3!, true);

            var context = results.Get("Namespace.ContextType");
            var behavior = results.Get("Namespace.ContextType.Namespace.ABehavior");

            Assert.False(context.IsSuccessful);
            Assert.True(behavior.IsSuccessful);
        }

        private IEnumerable<IMspecElement> GetElements()
        {
            var context = new ContextElement("Namespace.ContextType", "subject");
            var behavior = new BehaviorElement(context, "Namespace.ABehavior", "a_vehicle");
            var specification1 = new SpecificationElement(context, "should_be", behavior);
            var specification2 = new SpecificationElement(context, "should_not_be", behavior);
            var specification3 = new SpecificationElement(context, "should_be");

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
