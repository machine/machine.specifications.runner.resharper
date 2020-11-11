using JetBrains.Metadata.Reader.Impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.TestRunner;
using JetBrains.Util;
using Machine.Specifications.Runner.ReSharper.Elements;
using Machine.Specifications.Runner.ReSharper.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Mappings
{
    [SolutionComponent]
    public class MspecBehaviorSpecificationMapping : MspecElementMapping<BehaviorSpecificationElement, MspecBehaviorSpecificationRemoteTask>
    {
        public MspecBehaviorSpecificationMapping(MspecServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        protected override MspecBehaviorSpecificationRemoteTask ToRemoteTask(BehaviorSpecificationElement element, ITestRunnerExecutionContext context)
        {
            var task = MspecBehaviorSpecificationRemoteTask.ToClient(
                element.Id.Id,
                context.RunAllChildren(element),
                context.IsRunExplicitly(element));

            task.ContextTypeName = element.Behavior.Context.TypeName.FullName;
            task.SpecificationFieldName = element.FieldName;
            task.BehaviorFieldName = element.Behavior.FieldName;

            return task;
        }

        protected override BehaviorSpecificationElement ToElement(MspecBehaviorSpecificationRemoteTask task, ITestRunnerDiscoveryContext context)
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

            var contextId = ServiceProvider.CreateId(context.Project, context.TargetFrameworkId, task.ContextTypeName);
            var behaviorId = ServiceProvider.CreateId(context.Project, context.TargetFrameworkId, $"{task.ContextTypeName}::{task.BehaviorFieldName}");

            var contextElement = ServiceProvider.ElementManager.GetElementById<ContextElement>(contextId);
            var behavior = ServiceProvider.ElementManager.GetElementById<BehaviorElement>(behaviorId);

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
                task.SpecificationFieldName,
                false);
        }
    }
}
