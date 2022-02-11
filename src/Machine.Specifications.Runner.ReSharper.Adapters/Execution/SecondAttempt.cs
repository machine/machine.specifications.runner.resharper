using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Execution;

public class Results
{

}

public class ConcurrentLookup<T>
    where T : IMspecElement
{
    private readonly ConcurrentDictionary<string, ConcurrentBag<T>> values = new();

    public void Add(T? element)
    {
        if (element == null)
        {
            return;
        }

        var bag = values.GetOrAdd(element.GroupId, _ => new ConcurrentBag<T>());

        bag.Add(element);
    }

    public T? Take(string id)
    {
        if (values.TryGetValue(id, out var bag) && bag.TryTake(out var value))
        {
            return value;
        }

        return default;
    }
}

public class StartTracker
{
    private readonly ConcurrentLookup<IContextElement> pendingContexts = new();

    private readonly ConcurrentLookup<IContextElement> runningContexts = new();

    private readonly ConcurrentLookup<ISpecificationElement> pendingSpecifications = new();

    private readonly ConcurrentLookup<ISpecificationElement> runningSpecifications = new();

    public StartTracker(ISpecificationElement[] specifications)
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
            .Select(x => x.Behavior!.TypeName);

        behaviorTypes = new HashSet<string>(types);

        behaviorsByContext = specifications
            .Where(x => x.Behavior != null)
            .Select(x => x.Behavior!)
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

public class Listener : ISpecificationRunListener
{
    private readonly IExecutionListener listener;

    private readonly ElementCache cache;

    private readonly StartTracker tracker;

    private readonly HashSet<IMspecElement> behaviors = new();

    private IContextElement? currentContext;

    public Listener(IExecutionListener listener, ElementCache cache, StartTracker tracker)
    {
        this.listener = listener;
        this.cache = cache;
        this.tracker = tracker;
    }

    public void OnAssemblyStart(AssemblyInfo assemblyInfo)
    {
        listener.OnAssemblyStart(assemblyInfo.Location);
    }

    public void OnAssemblyEnd(AssemblyInfo assemblyInfo)
    {
        listener.OnAssemblyEnd(assemblyInfo.Location);
    }

    public void OnRunStart()
    {
        listener.OnRunStart();
    }

    public void OnRunEnd()
    {
        listener.OnRunEnd();
    }

    public void OnContextStart(ContextInfo contextInfo)
    {
        var context = tracker.StartContext(contextInfo.TypeName);

        currentContext = context;

        if (context != null)
        {
            listener.OnContextStart(context);
        }
    }

    public void OnContextEnd(ContextInfo contextInfo)
    {
        var context = tracker.FinishContext(contextInfo.TypeName);

        if (context != null)
        {
            var runningBehaviors = cache.GetBehaviors(context)
                .Where(x => behaviors.Contains(x));

            foreach (var behavior in runningBehaviors)
            {
                listener.OnBehaviorEnd(behavior);
            }

            listener.OnContextEnd(context);
        }
    }

    public void OnSpecificationStart(SpecificationInfo specificationInfo)
    {
        var isBehavior = cache.IsBehavior(specificationInfo.ContainingType);

        if (isBehavior)
        {
            var key = $"{currentContext?.TypeName}.{specificationInfo.ContainingType}.{specificationInfo.FieldName}";

            var specification = tracker.StartSpecification(key);

            if (specification?.Behavior != null && behaviors.Add(specification.Behavior))
            {
                listener.OnBehaviorStart(specification.Behavior);
            }

            if (specification != null)
            {
                listener.OnSpecificationStart(specification);
            }
        }
        else
        {
            var key = $"{specificationInfo.ContainingType}.{specificationInfo.FieldName}";

            var specification = tracker.StartSpecification(key);

            if (specification != null)
            {
                listener.OnSpecificationStart(specification);
            }
        }
    }

    public void OnSpecificationEnd(SpecificationInfo specificationInfo, Result result)
    {
        var isBehavior = cache.IsBehavior(specificationInfo.ContainingType);

        if (isBehavior)
        {
            var key = $"{currentContext?.TypeName}.{specificationInfo.ContainingType}.{specificationInfo.FieldName}";

            var specification = tracker.FinishSpecification(key);

            if (specification != null)
            {
                listener.OnSpecificationEnd(specification, result);
            }
        }
        else
        {
            var key = $"{specificationInfo.ContainingType}.{specificationInfo.FieldName}";

            var specification = tracker.FinishSpecification(key);

            if (specification != null)
            {
                listener.OnSpecificationEnd(specification, result);
            }
        }
    }

    public void OnFatalError(ExceptionResult exceptionResult)
    {
        listener.OnFatalError(exceptionResult);
    }
}
