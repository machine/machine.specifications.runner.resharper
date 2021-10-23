using System.Collections.Generic;
using System.Reflection;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using JetBrains.ReSharper.UnitTestFramework.Execution.TestRunner;
using JetBrains.ReSharper.UnitTestFramework.Execution.TestRunner.Extensions;
using JetBrains.Util;
using Machine.Specifications.Runner.ReSharper.Tasks;

namespace Machine.Specifications.Runner.ReSharper
{
    [SolutionComponent]
    public class MspecTestRunnerOrchestrator : ITestRunnerAdapter
    {
        private const string Namespace = "Machine.Specifications.Runner.ReSharper";

        private static readonly FileSystemPath Root = Assembly.GetExecutingAssembly().GetPath().Directory;

        public Assembly InProcessAdapterAssembly => typeof (MspecTestContainer).Assembly;

        public int Priority => 10;

        public TestAdapterLoader GetTestAdapterLoader(ITestRunnerContext context)
        {
            var framework = context.RuntimeEnvironment.TargetFrameworkId.IsNetCoreSdk()
                ? "netstandard20"
                : "net40";

            var adapters = Root.Combine($"{Namespace}.Adapters.{framework}.dll");
            var tasks = Root.Combine($"{Namespace}.Tasks.{framework}.dll");

            var type = TypeInfoFactory.Create($"{Namespace}.Adapters.MspecRunner", adapters.FullPath);

            return new TestAdapterInfo(type, type)
            {
                AdditionalAssemblies = new[]
                {
                    tasks.FullPath
                }
            };
        }

        public TestContainer GetTestContainer(ITestRunnerContext context)
        {
            return new MspecTestContainer(context.GetOutputPath().FullPath, context.Settings.TestRunner.ToShadowCopy());
        }

        public IEnumerable<IMessageHandlerMarker> GetMessageHandlers(ITestRunnerContext context)
        {
            yield break;
        }
    }
}
