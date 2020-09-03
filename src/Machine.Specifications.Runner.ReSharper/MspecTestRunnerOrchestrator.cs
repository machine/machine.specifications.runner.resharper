using System.Reflection;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using JetBrains.ReSharper.UnitTestFramework.Exploration;
using JetBrains.ReSharper.UnitTestFramework.Launch;
using JetBrains.ReSharper.UnitTestFramework.TestRunner;
using JetBrains.ReSharper.UnitTestFramework.TestRunner.Extensions;
using Machine.Specifications.Runner.ReSharper.Tasks;
using TypeInfo = JetBrains.ReSharper.TestRunner.Abstractions.Objects.TypeInfo;

namespace Machine.Specifications.Runner.ReSharper
{
    [SolutionComponent]
    public class MspecTestRunnerOrchestrator : TestRunnerOrchestrator
    {
        private const string Namespace = "Machine.Specifications.Runner.ReSharper";

        private readonly IUnitTestProjectArtifactResolver artifactResolver;

        public MspecTestRunnerOrchestrator(IUnitTestProjectArtifactResolver artifactResolver)
        {
            this.artifactResolver = artifactResolver;
        }

        public override Assembly AdapterAssembly { get; } = typeof(MspecAssemblyRemoteTask).Assembly;

        public override TestAdapterInfo GetTestAdapter(IUnitTestRun ctx)
        {
            var framework = ctx.GetEnvironment().TargetFrameworkId.IsNetCoreSdk()
                ? "netstandard2.0"
                : "net40";

            var suffix = framework.Replace(".", string.Empty);

            var adapters = MspecTestRunnerInfo.Root.Combine($"{Namespace}.Adapters.{suffix}.dll");
            var tasks = MspecTestRunnerInfo.Root.Combine($"{Namespace}.Tasks.{suffix}.dll");

            var type = new TypeInfo($"{Namespace}.Adapters.MspecRunner", adapters.FullPath);

            return new TestAdapterInfo(type, type)
            {
                AdditionalAssemblies = new[]
                {
                    tasks.FullPath
                }
            };
        }

        public override TestContainer GetTestContainer(IUnitTestRun ctx)
        {
            var outputPath = artifactResolver.GetArtifactFilePath(ctx.GetEnvironment().Project, ctx.TargetFrameworkId);

            return new MspecAssemblyRemoteTask(outputPath.FullPath, ctx.Launch.Settings.TestRunner.ToShadowCopy());
        }
    }
}
