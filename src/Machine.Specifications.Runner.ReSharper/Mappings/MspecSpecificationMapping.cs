using System.Linq;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.Launch;
using JetBrains.Util;
using Machine.Specifications.Runner.ReSharper.Adapters.Tasks;
using Machine.Specifications.Runner.ReSharper.Elements;

namespace Machine.Specifications.Runner.ReSharper.Mappings
{
    [SolutionComponent]
    public class MspecSpecificationMapping : MspecElementMapping<ContextSpecificationElement, MspecSpecificationRemoteTask>
    {
        public MspecSpecificationMapping(MspecServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        protected override MspecSpecificationRemoteTask ToRemoteTask(ContextSpecificationElement element, IUnitTestRun run)
        {
            var task = MspecSpecificationRemoteTask.ToClient(
                element.Id.Id,
                element.Children.All(x => run.Launch.Criterion.Criterion.Matches(x)),
                run.Launch.Criterion.Explicit.Contains(element));

            task.ContextTypeName = element.Context.TypeName.FullName;
            task.SpecificationFieldName = element.FieldName;

            return task;
        }

        protected override ContextSpecificationElement ToElement(MspecSpecificationRemoteTask task, IUnitTestRun run, IProject project, UnitTestElementFactory factory)
        {
            if (task.ContextTypeName == null)
            {
                run.Launch.Output.Warn("Cannot create element for ContextSpecificationElement '" + task.TestId + "': ContextTypeName is missing");

                return null;
            }

            var id = ServiceProvider.CreateId(project, run.TargetFrameworkId, task.ContextTypeName);

            var context = ServiceProvider.ElementManager.GetElementById<ContextElement>(id);

            if (context == null)
            {
                run.Launch.Output.Warn("Cannot create element for ContextSpecificationElement '" + task.TestId + "': Context is missing");
                return null;
            }

            return factory.GetOrCreateContextSpecification(
                project,
                context,
                new ClrTypeName(task.ContextTypeName),
                task.SpecificationFieldName,
                false);
        }
    }
}
