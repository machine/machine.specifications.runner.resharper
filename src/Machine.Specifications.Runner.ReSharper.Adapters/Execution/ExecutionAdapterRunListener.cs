using System.Collections.Generic;
using System.Linq;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.ReSharper.Adapters.Listeners;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Execution;

public class ExecutionAdapterRunListener : IRunListener
{
    private readonly IExecutionListener listener;

    private readonly ElementCache cache;

    private readonly RunTracker tracker;

    private readonly HashSet<IMspecElement> behaviors = new();

    private IContextElement? currentContext;

    public ExecutionAdapterRunListener(IExecutionListener listener, ElementCache cache, RunTracker tracker)
    {
        this.listener = listener;
        this.cache = cache;
        this.tracker = tracker;
    }

    public void OnAssemblyStart(TestAssemblyInfo assemblyInfo)
    {
        listener.OnAssemblyStart(assemblyInfo.Location);
    }

    public void OnAssemblyEnd(TestAssemblyInfo assemblyInfo)
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

    public void OnContextStart(TestContextInfo contextInfo)
    {
        var element = tracker.StartContext(contextInfo.TypeName);

        currentContext = element;

        if (element != null)
        {
            listener.OnContextStart(element);
        }
    }

    public void OnContextEnd(TestContextInfo contextInfo)
    {
        var element = tracker.FinishContext(contextInfo.TypeName);

        if (element != null)
        {
            var runningBehaviors = cache.GetBehaviors(element)
                .Where(x => behaviors.Contains(x));

            foreach (var behavior in runningBehaviors)
            {
                listener.OnBehaviorEnd(behavior, contextInfo.CapturedOutput);
            }

            listener.OnContextEnd(element, contextInfo.CapturedOutput);
        }

        currentContext = null;
    }

    public void OnSpecificationStart(TestSpecificationInfo specificationInfo)
    {
        var isBehavior = cache.IsBehavior(specificationInfo.ContainingType);

        if (isBehavior)
        {
            var key = $"{currentContext?.TypeName}.{specificationInfo.ContainingType}.{specificationInfo.FieldName}";

            var element = tracker.StartSpecification(key);

            if (element?.Behavior != null && behaviors.Add(element.Behavior))
            {
                listener.OnBehaviorStart(element.Behavior);
            }

            if (element != null)
            {
                listener.OnSpecificationStart(element);
            }
        }
        else
        {
            var key = $"{specificationInfo.ContainingType}.{specificationInfo.FieldName}";

            var element = tracker.StartSpecification(key);

            if (element != null)
            {
                listener.OnSpecificationStart(element);
            }
        }
    }

    public void OnSpecificationEnd(TestSpecificationInfo specificationInfo, TestRunResult runResult)
    {
        var isBehavior = cache.IsBehavior(specificationInfo.ContainingType);

        if (isBehavior)
        {
            var key = $"{currentContext?.TypeName}.{specificationInfo.ContainingType}.{specificationInfo.FieldName}";

            var element = tracker.FinishSpecification(key);

            if (element != null)
            {
                listener.OnSpecificationEnd(element, specificationInfo.CapturedOutput, runResult);
            }
        }
        else
        {
            var key = $"{specificationInfo.ContainingType}.{specificationInfo.FieldName}";

            var element = tracker.FinishSpecification(key);

            if (element != null)
            {
                listener.OnSpecificationEnd(element, specificationInfo.CapturedOutput, runResult);
            }
        }
    }

    public void OnFatalError(TestError? error)
    {
        listener.OnFatalError(error);
    }
}
