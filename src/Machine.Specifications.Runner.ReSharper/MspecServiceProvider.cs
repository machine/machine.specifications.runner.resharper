using System;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.Execution;
using JetBrains.Util;
using Machine.Specifications.Runner.ReSharper.Runner;

namespace Machine.Specifications.Runner.ReSharper
{
    [SolutionComponent]
    public class MspecServiceProvider
    {
        private readonly Lazy<MspecTestRunnerRunStrategy> runner;

        public MspecServiceProvider(MspecTestProvider provider, ISolution solution)
        {
            runner = Lazy.Of(solution.GetComponent<MspecTestRunnerRunStrategy>, true);

            Provider = provider;
        }

        public MspecTestProvider Provider { get; }

        public IUnitTestRunStrategy GetRunStrategy()
        {
            return runner.Value;
        }
    }
}
