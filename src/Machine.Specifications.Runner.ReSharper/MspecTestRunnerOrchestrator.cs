using System.Collections.Generic;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using JetBrains.ReSharper.UnitTestFramework.Execution.TestRunner;
using JetBrains.ReSharper.UnitTestFramework.Execution.TestRunner.Extensions;
using Machine.Specifications.Runner.ReSharper.Tasks;

namespace Machine.Specifications.Runner.ReSharper
{
    [SolutionComponent]
    public class MspecTestRunnerOrchestrator : ITestRunnerAdapter
    {
        private const string Namespace = "Machine.Specifications.Runner.ReSharper";

        public System.Reflection.Assembly InProcessAdapterAssembly => typeof (MspecTestContainer).Assembly;

        public int Priority => 10;

        public TestAdapterLoader GetTestAdapterLoader(ITestRunnerContext ctx)
        {
            var framework = ctx.RuntimeEnvironment.TargetFrameworkId.IsNetCoreSdk()
                ? "netstandard20"
                : "net40";

            var adapters = MspecTestRunnerInfo.Root.Combine($"{Namespace}.Adapters.{framework}.dll");
            var tasks = MspecTestRunnerInfo.Root.Combine($"{Namespace}.Tasks.{framework}.dll");

            var type = new TypeInfo($"{Namespace}.Adapters.MspecRunner", adapters.FullPath);

            return new TestAdapterInfo(type, type)
            {
                AdditionalAssemblies = new[]
                {
                    tasks.FullPath
                }
            };
        }

        public TestContainer GetTestContainer(ITestRunnerContext ctx)
        {
            return new MspecTestContainer(ctx.GetOutputPath().FullPath, ctx.Settings.TestRunner.ToShadowCopy());
        }

        public IEnumerable<IMessageHandlerMarker> GetMessageHandlers(ITestRunnerContext context)
        {
            yield break;
        }
    }
}
