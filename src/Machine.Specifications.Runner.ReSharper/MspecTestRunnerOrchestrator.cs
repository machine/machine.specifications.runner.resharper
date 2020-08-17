using System.Reflection;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using JetBrains.ReSharper.UnitTestFramework.Launch;
using JetBrains.ReSharper.UnitTestFramework.TestRunner;
using JetBrains.ReSharper.UnitTestFramework.TestRunner.Extensions;
using JetBrains.Util;
using Machine.Specifications.Runner.ReSharper.Adapters;
using TypeInfo = JetBrains.ReSharper.TestRunner.Abstractions.Objects.TypeInfo;

namespace Machine.Specifications.Runner.ReSharper
{
    [SolutionComponent]
    public class MspecTestRunnerOrchestrator : TestRunnerOrchestrator
    {
        public override Assembly AdapterAssembly { get; } = typeof(MspecTestRunnerOrchestrator).Assembly;

        public override TestAdapterInfo GetTestAdapter(IUnitTestRun ctx)
        {
            var path = typeof(MspecTestRunnerOrchestrator).Assembly.GetPath();
            var type = new TypeInfo("Machine.Specifications.Runner.ReSharper.Adapters.MspecRunner", path.FullPath);

            return new TestAdapterInfo(type, type);
        }

        public override TestContainer GetTestContainer(IUnitTestRun ctx)
        {
            var outputPath = GetEnvironment(ctx).Project.GetOutputFilePath(ctx.TargetFrameworkId);

            return new MspecAssemblyRemoteTask(outputPath.FullPath, ctx.Launch.Settings.TestRunner.ToShadowCopy());
        }

        private TestRunnerRuntimeEnvironment GetEnvironment(IUnitTestRun run)
        {
            return run.RuntimeEnvironment as TestRunnerRuntimeEnvironment;
        }
    }
}
