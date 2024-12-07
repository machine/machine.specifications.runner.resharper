using System;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.Execution;
using JetBrains.Util;
using Machine.Specifications.Runner.ReSharper.Runner;

namespace Machine.Specifications.Runner.ReSharper;

[SolutionComponent(Instantiation.ContainerAsyncPrimaryThread)]
public class MspecServiceProvider(ISolution solution)
{
    private readonly Lazy<MspecTestRunnerRunStrategy> runner = Lazy.Of(solution.GetComponent<MspecTestRunnerRunStrategy>, true);

    public IUnitTestRunStrategy GetRunStrategy()
    {
        return runner.Value;
    }
}
