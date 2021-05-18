using System.Diagnostics;
using System.IO;
using JetBrains.ReSharper.TaskRunnerFramework;
using Machine.Specifications.Runner.ReSharper.Runner.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Runner
{
    public class MspecTaskRunner : RecursiveRemoteTaskRunner
    {
        public const string RunnerId = "Machine.Specifications";

        public MspecTaskRunner(IRemoteTaskServer server)
            : base(server)
        {
        }

        public override void ExecuteRecursive(TaskExecutionNode node)
        {
#if DEBUG
            Debugger.Launch();
#endif

            foreach (var child in node.Children)
            {
                if (child.RemoteTask is MspecAssemblyRunnerTask assemblyTask)
                {
                    Execute(child, assemblyTask);
                }
            }
        }

        private void Execute(TaskExecutionNode node, MspecAssemblyRunnerTask assemblyTask)
        {
            var assemblyLocation = TaskExecutor.Configuration.GetAssemblyLocation(assemblyTask.AssemblyLocation);
            var directoryName = Path.GetDirectoryName(assemblyLocation);
            var config = Path.GetFullPath(assemblyLocation) + ".config";

            using var loader = new AssemblyLoader().WithDefaultTypes();
            using var shadowCopyCookie = ShadowCopy.SetupFor(directoryName);
            using var appDomain = AppDomainBuilder.Setup(assemblyLocation, config, shadowCopyCookie);
            using var unwrappedLoader = appDomain.CreateAndUnwrapFrom<AssemblyLoader>().WithDefaultTypes();

            unwrappedLoader.RegisterPathOf<TestRunner>();

            loader.RegisterPathOf<TestRunner>();
            loader.RegisterPath(shadowCopyCookie.TargetLocation);

            var runner = appDomain.CreateAndUnwrapFrom<TestRunner>();

            runner.Run(Server, TaskExecutor.Configuration, TaskExecutor.Logger, node, assemblyTask, shadowCopyCookie);
        }
    }
}
