using System.Linq;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.Launch;
using Machine.Specifications.Runner.ReSharper.Adapters;
using Machine.Specifications.Runner.ReSharper.Adapters.Tasks;
using Machine.Specifications.Runner.ReSharper.Elements;

namespace Machine.Specifications.Runner.ReSharper.Mappings
{
    [SolutionComponent]
    public class MspecTestContextMapping : MspecElementMapping<ContextElement, MspecTestContextRemoteTask>
    {
        public MspecTestContextMapping(MspecServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public override MspecTestContextRemoteTask ToRemoteTask(ContextElement element, IUnitTestRun run)
        {
            return MspecTestContextRemoteTask.ToClient(
                element.Id.Id,
                element.Children.All(x => run.Launch.Criterion.Criterion.Matches(x)),
                run.Launch.Criterion.Explicit.Contains(element));
        }

        public override ContextElement ToElement(MspecTestContextRemoteTask task, IUnitTestRun run)
        {
            var environment = run.GetEnvironment();

            return GetFactory(run).GetOrCreateContext(environment.Project, new ClrTypeName(task.TypeName), string.Empty, new string[0], false, out _);
        }
    }
}
