using System.Collections.Generic;
using System.Threading.Tasks;
using Machine.Specifications.Runner.ReSharper.Adapters.Discovery.Elements;
using Machine.Specifications.Runner.ReSharper.Adapters.Models;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Discovery
{
    public class MspecDiscoverySink : IMspecDiscoverySink
    {
        private readonly TaskCompletionSource<IMspecElement[]> source = new();

        private readonly List<IMspecElement> elements = new();

        private readonly HashSet<Specification> behaviors = new();

        public Task<IMspecElement[]> Elements => source.Task;

        public void OnContext(Context context)
        {
            elements.Add(context.AsContext());

            foreach (var specification in context.Specifications)
            {
                var contextSpecification = specification.AsSpecification();

                if (contextSpecification.IsBehavior && behaviors.Add(specification.SpecificationField))
                {
                    elements.Add(specification.SpecificationField.AsSpecification());
                }

                elements.Add(contextSpecification);
            }
        }

        public void OnDiscoveryCompleted()
        {
            source.SetResult(elements.ToArray());
        }
    }
}
