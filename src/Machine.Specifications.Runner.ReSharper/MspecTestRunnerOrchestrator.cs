using System.Reflection;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using JetBrains.ReSharper.UnitTestFramework.Launch;
using JetBrains.ReSharper.UnitTestFramework.TestRunner;
using JetBrains.ReSharper.UnitTestFramework.TestRunner.Extensions;
using Machine.Specifications.Runner.ReSharper.Adapters.Tasks;
using TypeInfo = JetBrains.ReSharper.TestRunner.Abstractions.Objects.TypeInfo;

namespace Machine.Specifications.Runner.ReSharper
{
    [SolutionComponent]
    public class MspecTestRunnerOrchestrator : TestRunnerOrchestrator
    {
        public override Assembly AdapterAssembly { get; } = typeof(MspecAssemblyRemoteTask).Assembly;

        public override TestAdapterInfo GetTestAdapter(IUnitTestRun ctx)
        {
            var framework = ctx.GetEnvironment().TargetFrameworkId.IsNetCoreSdk()
                ? "netstandard2.0"
                : "net40";

            var adapters = TestRunnerInfo.Directory.Adapters.Combine($"MSpec\\{framework}\\Machine.Specifications.Runner.ReSharper.Adapters.{framework.Replace(".", string.Empty)}.dll");

            var type = new TypeInfo("Machine.Specifications.Runner.ReSharper.Adapters.MspecRunner", adapters.FullPath);

            return new TestAdapterInfo(type, type);
        }

        public override TestContainer GetTestContainer(IUnitTestRun ctx)
        {
            var outputPath = ctx.GetEnvironment().Project.GetOutputFilePath(ctx.TargetFrameworkId);

            return new MspecAssemblyRemoteTask(outputPath.FullPath, ctx.Launch.Settings.TestRunner.ToShadowCopy());
        }
    }
}
