using System;
using System.Linq;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.Execution.TestRunner;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using Machine.Specifications.Runner.ReSharper.Elements;
using Machine.Specifications.Runner.ReSharper.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Mappings;

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
            element.NaturalId.TestId,
            element.IgnoreReason,
            context.RunAllChildren(element),
            context.IsRunExplicitly(element));

        task.Subject = element.Subject;
        task.Tags = element.OwnCategories?.Select(x => x.Name).ToArray() ?? Array.Empty<string>();

        return task;
    }

    protected override MspecContextTestElement ToElement(MspecContextRemoteTask task, ITestRunnerDiscoveryContext context, IUnitTestElementObserver observer)
    {
        var factory = GetFactory(context);

        return factory.GetOrCreateContext(
            new ClrTypeName(task.ContextTypeName),
            task.Subject,
            task.Tags,
            task.IgnoreReason);
    }
}
