using JetBrains.Metadata.Reader.Impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.TestRunner;
using JetBrains.Util;
using Machine.Specifications.Runner.ReSharper.Elements;
using Machine.Specifications.Runner.ReSharper.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Mappings
{
    [SolutionComponent]
    public class MspecSpecificationMapping : MspecElementMapping<ContextSpecificationElement, MspecContextSpecificationRemoteTask>
    {
        public MspecSpecificationMapping(MspecServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        protected override MspecContextSpecificationRemoteTask ToRemoteTask(ContextSpecificationElement element, ITestRunnerExecutionContext context)
        {
            var task = MspecContextSpecificationRemoteTask.ToClient(
                element.Id.Id,
                context.RunAllChildren(element),
                context.IsRunExplicitly(element));

            task.ContextTypeName = element.Context.TypeName.FullName;
            task.SpecificationFieldName = element.FieldName;

            return task;
        }

        protected override ContextSpecificationElement ToElement(MspecContextSpecificationRemoteTask task, ITestRunnerDiscoveryContext context)
        {
            if (task.ContextTypeName == null)
            {
                context.Logger.Warn("Cannot create element for ContextSpecificationElement '" + task.TestId + "': ContextTypeName is missing");

                return null;
            }

            var id = ServiceProvider.CreateId(context.Project, context.TargetFrameworkId, task.ContextTypeName);

            var contextElement = ServiceProvider.ElementManager.GetElementById<ContextElement>(id);

            if (contextElement == null)
            {
                context.Logger.Warn("Cannot create element for ContextSpecificationElement '" + task.TestId + "': Context is missing");
                return null;
            }

            var factory = GetFactory(context);

            return factory.GetOrCreateContextSpecification(
                context.Project,
                contextElement,
                new ClrTypeName(task.ContextTypeName),
                task.SpecificationFieldName,
                false);
        }
    }
}
