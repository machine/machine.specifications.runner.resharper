using System.Linq;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.Launch;
using JetBrains.Util;
using Machine.Specifications.Runner.ReSharper.Adapters;
using Machine.Specifications.Runner.ReSharper.Elements;

namespace Machine.Specifications.Runner.ReSharper.Mappings
{
    [SolutionComponent]
    public class MspecTestSpecificationMapping : MspecElementMapping<ContextSpecificationElement, MspecTestSpecificationRemoteTask>
    {
        public MspecTestSpecificationMapping(MspecServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public override MspecTestSpecificationRemoteTask ToRemoteTask(ContextSpecificationElement element, IUnitTestRun run)
        {
            return MspecTestSpecificationRemoteTask.ToClient(
                element.Id.Id,
                element.Children.All(x => run.Launch.Criterion.Criterion.Matches(x)),
                run.Launch.Criterion.Explicit.Contains(element));
        }

        public override ContextSpecificationElement ToElement(MspecTestSpecificationRemoteTask task, IUnitTestRun run)
        {
            if (task.ParentId == null)
            {
                run.Launch.Output.Warn("Cannot create element for ContextSpecificationElement '" + task.TestId + "': ParentId is missing");

                return null;
            }

            var environment = run.GetEnvironment();
            var factory = GetFactory(run);

            var id = ServiceProvider.CreateId(environment.Project, run.TargetFrameworkId, task.ParentId);

            var context = ServiceProvider.ElementManager.GetElementById<ContextElement>(id);

            if (context == null)
            {
                run.Launch.Output.Warn("Cannot create element for ContextSpecificationElement '" + task.TestId + "': ParentId is missing");
                return null;
            }

            return factory.GetOrCreateContextSpecification(
                environment.Project,
                context,
                new ClrTypeName(task.ContextTypeName),
                task.SpecificationFieldName,
                false);
        }
    }
}
