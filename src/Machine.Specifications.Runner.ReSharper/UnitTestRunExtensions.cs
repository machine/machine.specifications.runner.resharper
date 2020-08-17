using JetBrains.ReSharper.UnitTestFramework.Launch;
using JetBrains.ReSharper.UnitTestFramework.TestRunner;

namespace Machine.Specifications.Runner.ReSharper
{
    public static class UnitTestRunExtensions
    {
        public static TestRunnerRuntimeEnvironment GetEnvironment(this IUnitTestRun run)
        {
            return run.RuntimeEnvironment as TestRunnerRuntimeEnvironment;
        }
    }
}
