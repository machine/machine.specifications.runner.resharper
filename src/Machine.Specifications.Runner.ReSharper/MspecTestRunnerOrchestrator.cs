using System.Collections.Generic;
using System.Reflection;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using JetBrains.ReSharper.UnitTestFramework.TestRunner;
using JetBrains.ReSharper.UnitTestFramework.TestRunner.Extensions;
using Machine.Specifications.Runner.ReSharper.Tasks;
using TypeInfo = JetBrains.ReSharper.TestRunner.Abstractions.Objects.TypeInfo;

namespace Machine.Specifications.Runner.ReSharper
{
    [SolutionComponent]
    public class MspecTestRunnerOrchestrator : ITestRunnerAdapter
    {
        private const string Namespace = "Machine.Specifications.Runner.ReSharper";

        public Assembly InProcessAdapterAssembly => typeof (MspecTestContainer).Assembly;

        public int Priority => 10;

        public TestAdapterLoader GetTestAdapterLoader(ITestRunnerContext ctx)
        {
            var framework = ctx.RuntimeEnvironment.TargetFrameworkId.IsNetCoreSdk()
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
