using System.Linq;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Execution
{
    public class RunTracker
    {
        private readonly ConcurrentLookup<IContextElement> pendingContexts = new();

        private readonly ConcurrentLookup<IContextElement> runningContexts = new();

        private readonly ConcurrentLookup<ISpecificationElement> pendingSpecifications = new();

        private readonly ConcurrentLookup<ISpecificationElement> runningSpecifications = new();

        public RunTracker(ISpecificationElement[] specifications)
        {
            var contexts = specifications
                .Select(x => x.Context)
                .Distinct();

            foreach (var context in contexts)
            {
                pendingContexts.Add(context);
            }

            foreach (var specification in specifications)
            {
                pendingSpecifications.Add(specification);
            }
        }

        public IContextElement? StartContext(string id)
        {
            var element = pendingContexts.Take(id);

            runningContexts.Add(element);

            return element;
        }

        public ISpecificationElement? StartSpecification(string id)
        {
            var element = pendingSpecifications.Take(id);

            runningSpecifications.Add(element);

            return element;
        }

        public IContextElement? FinishContext(string id)
        {
            return runningContexts.Take(id);
        }

        public ISpecificationElement? FinishSpecification(string id)
        {
            return runningSpecifications.Take(id);
        }
    }
}
