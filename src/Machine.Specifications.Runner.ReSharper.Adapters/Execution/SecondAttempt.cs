using System.Collections.Concurrent;
using System.Collections.Generic;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Execution;

public class Results
{

}

public class ConcurrentLookup
{
    private ConcurrentDictionary<string, ConcurrentBag<IMspecElement>> values;

    public void Add(IMspecElement element)
    {
        var bag = values.GetOrAdd(element.GroupId, _ => new ConcurrentBag<IMspecElement>());

        bag.Add(element);
    }

    public IMspecElement? Take(string id)
    {
        if (values.TryGetValue(id, out var bag) && bag.TryTake(out var value))
        {
            return value;
        }

        return null;
    }
}

public class StartTracker
{
    private Dictionary<string, IMspecElement> elements;

    private ConcurrentLookup pending;

    private ConcurrentLookup running;

    private HashSet<IMspecElement> started;

    public IMspecElement Get(string id)
    {
        return elements[id];
    }

    public IMspecElement? Started(string id)
    {
        var element = pending.Take(id);

        if (element != null)
        {
            running.Add(element);
        }

        return element;
    }

    public void Finished(string id)
    {
        running.Take(id);
    }
}

public class Listener : ISpecificationRunListener
{
    private readonly IExecutionListener listener;

    private readonly StartTracker tracker;

    private IContextElement currentContext;

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
        tracker.Started(contextInfo.TypeName);
    }

    public void OnContextEnd(ContextInfo contextInfo)
    {
        tracker.Finished(contextInfo.TypeName);
    }

    public void OnSpecificationStart(SpecificationInfo specificationInfo)
    {
    }

    public void OnSpecificationEnd(SpecificationInfo specificationInfo, Result result)
    {
    }

    public void OnFatalError(ExceptionResult exceptionResult)
    {
    }
}
