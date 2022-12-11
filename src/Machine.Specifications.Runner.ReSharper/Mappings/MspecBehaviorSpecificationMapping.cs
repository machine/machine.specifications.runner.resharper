using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.Execution.TestRunner;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.Util;
using Machine.Specifications.Runner.ReSharper.Elements;
using Machine.Specifications.Runner.ReSharper.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Mappings;

[SolutionComponent]
public class MspecBehaviorSpecificationMapping : MspecElementMapping<MspecBehaviorSpecificationTestElement, MspecBehaviorSpecificationRemoteTask>
{
    public MspecBehaviorSpecificationMapping(MspecServiceProvider services)
        : base(services)
    {
    }

    protected override MspecBehaviorSpecificationRemoteTask ToRemoteTask(MspecBehaviorSpecificationTestElement element, ITestRunnerExecutionContext context)
    {
        var task = MspecBehaviorSpecificationRemoteTask.ToClient(
            element.NaturalId.TestId,
            element.IgnoreReason,
            context.IsRunExplicitly(element));

        task.ContextTypeName = element.Specification.Context.TypeName.FullName;
        task.FieldName = element.FieldName;
        task.ParentId = $"{element.Specification.Context.TypeName.FullName}.{element.Specification.FieldName}";

        return task;
    }

    protected override MspecBehaviorSpecificationTestElement? ToElement(MspecBehaviorSpecificationRemoteTask task, ITestRunnerDiscoveryContext context, IUnitTestElementObserver observer)
    {
        if (task.ParentId == null)
        {
            context.Logger.Warn($"Cannot create element for MspecBehaviorSpecificationTestElement '{task.TestId}': ParentId is missing");

            return null;
        }

        var specificationElement = observer.GetElementById<MspecSpecificationTestElement>(task.ParentId);

        if (specificationElement == null)
        {
            context.Logger.Warn("Cannot create element for MspecBehaviorSpecificationTestElement '" + task.TestId + "': Specification is missing");

            return null;
        }

        var factory = GetFactory(context);

        return factory.GetOrCreateBehaviorSpecification(
            specificationElement,
            task.FieldName!,
            task.IgnoreReason);
    }
}
