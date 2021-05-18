using JetBrains.Metadata.Reader.Impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.TestRunner;
using JetBrains.Util;
using Machine.Specifications.Runner.ReSharper.Elements;
using Machine.Specifications.Runner.ReSharper.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Mappings
{
    [SolutionComponent]
    public class MspecBehaviorMapping : MspecElementMapping<MspecBehaviorTestElement, MspecBehaviorRemoteTask>
    {
        public MspecBehaviorMapping(MspecServiceProvider services)
            : base(services)
        {
        }

        protected override MspecBehaviorRemoteTask ToRemoteTask(MspecBehaviorTestElement element, ITestRunnerExecutionContext context)
        {
            var task = MspecBehaviorRemoteTask.ToClient(
                element.Id.Id,
                element.ExplicitReason,
                context.RunAllChildren(element),
                context.IsRunExplicitly(element));

            task.ContextTypeName = element.Context!.TypeName.FullName;
            task.BehaviorFieldName = element.FieldName;

            return task;
        }

        protected override MspecBehaviorTestElement? ToElement(MspecBehaviorRemoteTask task, ITestRunnerDiscoveryContext context)
        {
            if (task.ContextTypeName == null)
            {
                context.Logger.Warn("Cannot create element for BehaviorElement '" + task.TestId + "': ContextTypeName is missing");

                return null;
            }

            var contextId = Services.CreateId(context.Project, context.TargetFrameworkId, task.ContextTypeName);
            var contextElement = Services.ElementManager.GetElementById<MspecContextTestElement>(contextId);

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
                task.BehaviorFieldName!,
                null);
        }
    }
}
