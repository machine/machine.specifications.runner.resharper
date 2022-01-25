using System.Collections.Generic;
using System.Threading.Tasks;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Discovery
{
    public class MspecDiscoverySink : IMspecDiscoverySink
    {
        private readonly TaskCompletionSource<IMspecElement[]> source = new();

        private readonly List<IMspecElement> elements = new();

        private readonly HashSet<IContextElement> handledContexts = new();

        private readonly HashSet<IBehaviorElement> handledBehaviors = new();

        private readonly HashSet<ISpecificationElement> handledSpecifications = new();

        public Task<IMspecElement[]> Elements => source.Task;

        public void OnSpecification(ISpecificationElement specification)
        {
            if (handledContexts.Add(specification.Context))
            {
                elements.Add(specification.Context);
            }

            if (specification.Behavior != null && handledBehaviors.Add(specification.Behavior))
            {
                elements.Add(specification.Behavior);
            }

            if (handledSpecifications.Add(specification))
            {
                elements.Add(specification);
            }
        }

        public void OnDiscoveryCompleted()
        {
            source.SetResult(elements.ToArray());
        }
    }
}
