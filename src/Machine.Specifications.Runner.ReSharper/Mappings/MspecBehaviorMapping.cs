using System.Linq;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.Launch;
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

        protected override MspecBehaviorRemoteTask ToRemoteTask(BehaviorElement element, IUnitTestRun run)
        {
            var elements = run.Elements.ToSet();

            var task = MspecBehaviorRemoteTask.ToClient(
                element.Id.Id,
                element.Children.All(x => elements.Contains(x)),
                run.Launch.Criterion.Explicit.Contains(element));

            task.ContextTypeName = element.Context.TypeName.FullName;
            task.BehaviorFieldName = element.FieldName;

            return task;
        }

        protected override BehaviorElement ToElement(MspecBehaviorRemoteTask task, IUnitTestRun run, IProject project, UnitTestElementFactory factory)
        {
            if (task.ContextTypeName == null)
            {
                run.Launch.Output.Warn("Cannot create element for BehaviorElement '" + task.TestId + "': ContextTypeName is missing");

                return null;
            }

            var contextId = ServiceProvider.CreateId(project, run.TargetFrameworkId, task.ContextTypeName);
            var context = ServiceProvider.ElementManager.GetElementById<ContextElement>(contextId);

            if (context == null)
            {
                run.Launch.Output.Warn("Cannot create element for BehaviorElement '" + task.TestId + "': Context is missing");
                return null;
            }

            return factory.GetOrCreateBehavior(
                project,
                context,
                new ClrTypeName(task.ContextTypeName),
                task.BehaviorFieldName,
                false);
        }
    }
}
