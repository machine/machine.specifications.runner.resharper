using System;
using System.Linq;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.Launch;
using Machine.Specifications.Runner.ReSharper.Elements;
using Machine.Specifications.Runner.ReSharper.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Mappings
{
    [SolutionComponent]
    public class MspecContextMapping : MspecElementMapping<ContextElement, MspecContextRemoteTask>
    {
        public MspecContextMapping(MspecServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        protected override MspecContextRemoteTask ToRemoteTask(ContextElement element, IUnitTestRun run)
        {
            var elements = run.Elements.ToSet();

            var task = MspecContextRemoteTask.ToClient(
                element.Id.Id,
                element.Children.All(x => elements.Contains(x)),
                run.Launch.Criterion.Explicit.Contains(element));

            task.Subject = element.Subject;
            task.Tags = element.OwnCategories?.Select(x => x.Name).ToArray() ?? Array.Empty<string>();

            return task;
        }

        protected override ContextElement ToElement(MspecContextRemoteTask task, IUnitTestRun run, IProject project, UnitTestElementFactory factory)
        {
            return factory.GetOrCreateContext(
                project,
                new ClrTypeName(task.ContextTypeName),
                task.Subject,
                task.Tags,
                false,
                out _);
        }
    }
}
