using System;
using JetBrains.ReSharper.TaskRunnerFramework;
using Machine.Specifications.Runner.ReSharper.Runner.Tasks;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Runner
{
    public class TestRunner : MarshalByRefObject
    {
        public void Run(
            IRemoteTaskServer server,
            TaskExecutorConfiguration configuration,
            SimpleLogger logger,
            TaskExecutionNode node,
            MspecAssemblyTask assemblyTask,
            IShadowCopyCookie shadowCopy)
        {
            TaskExecutor.Configuration = configuration;
            TaskExecutor.Logger = logger;

            var environment = new TestEnvironment(assemblyTask, shadowCopy);
            var context = new RunContext(node);

            var listener = new TestRunListener(server, context);

            var runOptions = RunOptions.Custom.FilterBy(context.GetContextNames());

            var appDomainRunner = new AppDomainRunner(listener, runOptions);

            try
            {
                appDomainRunner.RunAssembly(new AssemblyPath(environment.AssemblyPath));
            }
            catch (Exception ex)
            {
                server.ShowNotification("Unable to run tests: " + ex.Message, string.Empty);
                server.TaskException(null, new[] {new TaskException(ex)});
            }
        }
    }
}
