using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.Execution.TestRunner;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.Util;
using Machine.Specifications.Runner.ReSharper.Elements;
using Machine.Specifications.Runner.ReSharper.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Mappings
{
    [SolutionComponent]
    public class MspecContextSpecificationMapping : MspecElementMapping<MspecContextSpecificationTestElement, MspecContextSpecificationRemoteTask>
    {
        public MspecContextSpecificationMapping(MspecServiceProvider services)
            : base(services)
        {
        }

        protected override MspecContextSpecificationRemoteTask ToRemoteTask(MspecContextSpecificationTestElement element, ITestRunnerExecutionContext context)
        {
            var task = MspecContextSpecificationRemoteTask.ToClient(
                element.NaturalId.TestId,
                element.IgnoreReason,
                context.RunAllChildren(element),
                context.IsRunExplicitly(element));

            task.ContextTypeName = element.Context.TypeName.FullName;
            task.SpecificationFieldName = element.FieldName;

            return task;
        }

        protected override MspecContextSpecificationTestElement? ToElement(MspecContextSpecificationRemoteTask task, ITestRunnerDiscoveryContext context, IUnitTestElementObserver observer)
        {
            if (task.ContextTypeName == null)
            {
                context.Logger.Warn($"Cannot create element for MspecContextSpecificationTestElement '{task.TestId}': ContextTypeName is missing");

                return null;
            }

            var factory = GetFactory(context);

            var contextElement = observer.GetElementById<MspecContextTestElement>(task.ContextTypeName);

            if (contextElement == null)
            {
                context.Logger.Warn($"Cannot create element for ContextSpecificationElement '{task.TestId}': Context is missing");

                return null;
            }

            return factory.GetOrCreateContextSpecification(
                contextElement,
                task.SpecificationFieldName!,
                task.BehaviorType,
                task.IgnoreReason);
        }
    }
}
