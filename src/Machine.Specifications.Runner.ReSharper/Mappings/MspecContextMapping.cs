using System;
using System.Linq;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.TestRunner;
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

        protected override MspecContextRemoteTask ToRemoteTask(ContextElement element, ITestRunnerExecutionContext context)
        {
            var task = MspecContextRemoteTask.ToClient(
                element.Id.Id,
                context.RunAllChildren(element),
                context.IsRunExplicitly(element));

            task.Subject = element.Subject;
            task.Tags = element.OwnCategories?.Select(x => x.Name).ToArray() ?? Array.Empty<string>();

            return task;
        }

        protected override ContextElement ToElement(MspecContextRemoteTask task, ITestRunnerDiscoveryContext context)
        {
            var factory = GetFactory(context);

            return factory.GetOrCreateContext(
                context.Project,
                new ClrTypeName(task.ContextTypeName),
                task.Subject,
                task.Tags,
                false,
                out _);
        }
    }
}
