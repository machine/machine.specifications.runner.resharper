using JetBrains.Metadata.Reader.Impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.TestRunner;
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
                element.Id.Id,
                element.ExplicitReason,
                context.RunAllChildren(element),
                context.IsRunExplicitly(element));

            task.ContextTypeName = element.Behavior!.Context!.TypeName.FullName;
            task.SpecificationFieldName = element.FieldName;
            task.BehaviorFieldName = element.Behavior.FieldName;

            return task;
        }

        protected override MspecBehaviorSpecificationTestElement? ToElement(MspecBehaviorSpecificationRemoteTask task, ITestRunnerDiscoveryContext context)
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

            var contextId = Services.CreateId(context.Project, context.TargetFrameworkId, task.ContextTypeName);
            var behaviorId = Services.CreateId(context.Project, context.TargetFrameworkId, $"{task.ContextTypeName}::{task.BehaviorFieldName}");

            var contextElement = Services.ElementManager.GetElementById<MspecContextTestElement>(contextId);
            var behavior = Services.ElementManager.GetElementById<MspecBehaviorTestElement>(behaviorId);

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
                context.Project,
                behavior,
                new ClrTypeName(task.ContextTypeName),
                task.SpecificationFieldName!,
                null);
        }
    }
}
