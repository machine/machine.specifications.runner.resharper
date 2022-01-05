using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.Execution.TestRunner;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.Util;
using Machine.Specifications.Runner.ReSharper.Elements;
using Machine.Specifications.Runner.ReSharper.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Mappings
{
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
                context.RunAllChildren(element),
                context.IsRunExplicitly(element));

            task.ContextTypeName = element.Specification.Context.TypeName.FullName;
            task.SpecificationFieldName = element.FieldName;
            task.BehaviorType = element.BehaviorType;
            task.ParentId = $"{element.Specification.Context.TypeName.FullName}.{element.Specification.FieldName}";

            return task;
        }

        protected override MspecBehaviorSpecificationTestElement? ToElement(MspecBehaviorSpecificationRemoteTask task, ITestRunnerDiscoveryContext context, IUnitTestElementObserver observer)
        {
            if (task.ContextTypeName == null)
            {
                context.Logger.Warn($"Cannot create element for BehaviorSpecificationElement '{task.TestId}': ContextTypeName is missing");

                return null;
            }

            if (task.ParentId == null)
            {
                context.Logger.Warn($"Cannot create element for BehaviorSpecificationElement '{task.TestId}': ParentId is missing");

                return null;
            }

            var specificationElement = observer.GetElementById<MspecContextSpecificationTestElement>(task.ParentId);

            if (specificationElement == null)
            {
                context.Logger.Warn("Cannot create element for BehaviorSpecificationElement '" + task.TestId + "': Specification is missing");

                return null;
            }

            var factory = GetFactory(context);

            return factory.GetOrCreateBehaviorSpecification(
                specificationElement,
                task.SpecificationFieldName!,
                task.BehaviorType,
                null);
        }
    }
}
