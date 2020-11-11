using JetBrains.Metadata.Reader.Impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.TestRunner;
using JetBrains.Util;
using Machine.Specifications.Runner.ReSharper.Elements;
using Machine.Specifications.Runner.ReSharper.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Mappings
{
    [SolutionComponent]
    public class MspecBehaviorMapping : MspecElementMapping<BehaviorElement, MspecBehaviorRemoteTask>
    {
        public MspecBehaviorMapping(MspecServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        protected override MspecBehaviorRemoteTask ToRemoteTask(BehaviorElement element, ITestRunnerExecutionContext context)
        {
            var task = MspecBehaviorRemoteTask.ToClient(
                element.Id.Id,
                context.RunAllChildren(element),
                context.IsRunExplicitly(element));

            task.ContextTypeName = element.Context.TypeName.FullName;
            task.BehaviorFieldName = element.FieldName;

            return task;
        }

        protected override BehaviorElement ToElement(MspecBehaviorRemoteTask task, ITestRunnerDiscoveryContext context)
        {
            if (task.ContextTypeName == null)
            {
                context.Logger.Warn("Cannot create element for BehaviorElement '" + task.TestId + "': ContextTypeName is missing");

                return null;
            }

            var contextId = ServiceProvider.CreateId(context.Project, context.TargetFrameworkId, task.ContextTypeName);
            var contextElement = ServiceProvider.ElementManager.GetElementById<ContextElement>(contextId);

            if (contextElement == null)
            {
                context.Logger.Warn("Cannot create element for BehaviorElement '" + task.TestId + "': Context is missing");
                return null;
            }

            var factory = GetFactory(context);

            return factory.GetOrCreateBehavior(
                context.Project,
                contextElement,
                new ClrTypeName(task.ContextTypeName),
                task.BehaviorFieldName,
                false);
        }
    }
}
