using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.Execution.TestRunner;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.Util;
using Machine.Specifications.Runner.ReSharper.Elements;
using Machine.Specifications.Runner.ReSharper.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Mappings
{
    [SolutionComponent]
    public class MspecSpecificationMapping : MspecElementMapping<MspecSpecificationTestElement, MspecSpecificationRemoteTask>
    {
        public MspecSpecificationMapping(MspecServiceProvider services)
            : base(services)
        {
        }

        protected override MspecSpecificationRemoteTask ToRemoteTask(MspecSpecificationTestElement element, ITestRunnerExecutionContext context)
        {
            var task = MspecSpecificationRemoteTask.ToClient(
                element.NaturalId.TestId,
                element.IgnoreReason,
                context.RunAllChildren(element),
                context.IsRunExplicitly(element));

            task.ContextTypeName = element.Context.TypeName.FullName;
            task.FieldName = element.FieldName;
            task.BehaviorType = element.BehaviorType;

            return task;
        }

        protected override MspecSpecificationTestElement? ToElement(MspecSpecificationRemoteTask task, ITestRunnerDiscoveryContext context, IUnitTestElementObserver observer)
        {
            if (task.ContextTypeName == null)
            {
                context.Logger.Warn($"Cannot create element for MspecSpecificationTestElement '{task.TestId}': ContextTypeName is missing");

                return null;
            }

            var factory = GetFactory(context);

            var contextElement = observer.GetElementById<MspecContextTestElement>(task.ContextTypeName);

            if (contextElement == null)
            {
                context.Logger.Warn($"Cannot create element for MspecSpecificationTestElement '{task.TestId}': Context is missing");

                return null;
            }

            return factory.GetOrCreateSpecification(
                contextElement,
                task.FieldName!,
                task.BehaviorType,
                task.IgnoreReason);
        }
    }
}
