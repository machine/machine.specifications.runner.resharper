using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.Execution.TestRunner;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
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
                element.NaturalId.TestId,
                element.IgnoreReason,
                context.RunAllChildren(element),
                context.IsRunExplicitly(element));

            task.ContextTypeName = element.Context.TypeName.FullName;
            task.BehaviorFieldName = element.FieldName;

            return task;
        }

        protected override MspecBehaviorTestElement? ToElement(MspecBehaviorRemoteTask task, ITestRunnerDiscoveryContext context, IUnitTestElementObserver observer)
        {
            if (task.ContextTypeName == null)
            {
                context.Logger.Warn("Cannot create element for BehaviorElement '" + task.TestId + "': ContextTypeName is missing");

                return null;
            }

            var contextElement = observer.GetElementById<MspecContextTestElement>(task.ContextTypeName);

            if (contextElement == null)
            {
                context.Logger.Warn("Cannot create element for BehaviorElement '" + task.TestId + "': Context is missing");
                return null;
            }

            var factory = GetFactory(context);

            return factory.GetOrCreateBehavior(
                contextElement,
                task.BehaviorFieldName!,
                task.IgnoreReason);
        }
    }
}
