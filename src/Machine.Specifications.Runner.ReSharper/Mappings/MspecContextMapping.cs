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
    public class MspecContextMapping : MspecElementMapping<MspecContextTestElement, MspecContextRemoteTask>
    {
        public MspecContextMapping(MspecServiceProvider services)
            : base(services)
        {
        }

        protected override MspecContextRemoteTask ToRemoteTask(MspecContextTestElement element, ITestRunnerExecutionContext context)
        {
            var task = MspecContextRemoteTask.ToClient(
                element.Id.Id,
                element.ExplicitReason,
                context.RunAllChildren(element),
                context.IsRunExplicitly(element));

            task.Subject = element.Subject;
            task.Tags = element.OwnCategories?.Select(x => x.Name).ToArray() ?? Array.Empty<string>();

            return task;
        }

        protected override MspecContextTestElement ToElement(MspecContextRemoteTask task, ITestRunnerDiscoveryContext context)
        {
            var factory = GetFactory(context);

            return factory.GetOrCreateContext(
                context.Project,
                new ClrTypeName(task.ContextTypeName),
                task.Subject,
                task.Tags,
                null);
        }
    }
}
