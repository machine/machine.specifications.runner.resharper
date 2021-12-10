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

            task.ContextTypeName = element.Behavior!.Context.TypeName.FullName;
            task.SpecificationFieldName = element.FieldName;
            task.BehaviorFieldName = element.Behavior.FieldName;

            return task;
        }

        protected override MspecBehaviorSpecificationTestElement? ToElement(MspecBehaviorSpecificationRemoteTask task, ITestRunnerDiscoveryContext context, IUnitTestElementObserver observer)
        {
            if (task.ContextTypeName == null)
            {
                context.Logger.Warn("Cannot create element for BehaviorSpecificationElement '" + task.TestId + "': ContextTypeName is missing");

                return null;
            }

            if (task.BehaviorFieldName == null)
            {
                context.Logger.Warn("Cannot create element for BehaviorSpecificationElement '" + task.TestId + "': BehaviorFieldName is missing");

                return null;
            }

            var contextElement = observer.GetElementById<MspecContextTestElement>(task.ContextTypeName);
            var behavior = observer.GetElementById<MspecBehaviorTestElement>($"{task.ContextTypeName}.{task.BehaviorFieldName}");

            if (contextElement == null)
            {
                context.Logger.Warn("Cannot create element for BehaviorSpecificationElement '" + task.TestId + "': Context is missing");

                return null;
            }

            if (behavior == null)
            {
                context.Logger.Warn("Cannot create element for BehaviorSpecificationElement '" + task.TestId + "': Behavior is missing");

                return null;
            }

            var factory = GetFactory(context);

            return factory.GetOrCreateBehaviorSpecification(
                behavior,
                task.SpecificationFieldName!,
                null);
        }
    }
}
