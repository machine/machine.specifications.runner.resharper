using System.Collections.Generic;
using System.Linq;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Execution;

public class ElementCache
{
    private readonly HashSet<string> behaviorTypes;

    private readonly ILookup<IContextElement, IBehaviorElement> behaviorsByContext;

    private readonly ILookup<IContextElement, ISpecificationElement> specificationsByContext;

    private readonly ILookup<IBehaviorElement, ISpecificationElement> specificationsByBehavior;

    public ElementCache(ISpecificationElement[] specifications)
    {
        var types = specifications
            .Where(x => x.Behavior != null)
            .Select(x => x.Behavior!.TypeName)
            .Distinct();

        behaviorTypes = new HashSet<string>(types);

        behaviorsByContext = specifications
            .Where(x => x.Behavior != null)
            .Select(x => x.Behavior!)
            .Distinct()
            .ToLookup(x => x!.Context);

        specificationsByContext = specifications
            .ToLookup(x => x.Context);

        specificationsByBehavior = specifications
            .Where(x => x.Behavior != null)
            .ToLookup(x => x.Behavior!);
    }

    public bool IsBehavior(string type)
    {
        return behaviorTypes.Contains(type);
    }

    public IEnumerable<IBehaviorElement> GetBehaviors(IContextElement element)
    {
        return behaviorsByContext[element];
    }

    public IEnumerable<ISpecificationElement> GetSpecifications(IContextElement element)
    {
        return specificationsByContext[element];
    }

    public IEnumerable<ISpecificationElement> GetSpecifications(IBehaviorElement behavior)
    {
        return specificationsByBehavior[behavior];
    }
}
