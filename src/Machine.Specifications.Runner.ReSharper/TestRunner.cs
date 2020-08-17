using System;
using JetBrains.ReSharper.TaskRunnerFramework;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper
{
    public class TestRunner : MarshalByRefObject
    {
        private readonly IRemoteTaskServer taskServer;

        public TestRunner(IRemoteTaskServer taskServer)
        {
            this.taskServer = taskServer;
        }

        public void Run(TestContext context)
        {
            var environment = new TestEnvironment(context.AssemblyTask);
            var listener = new TestRunListener(taskServer, context);

            var runOptions = RunOptions.Custom.FilterBy(context.GetContextNames());

            if (environment.ShouldShadowCopy)
            {
                runOptions.ShadowCopyTo(environment.ShadowCopyPath);
                taskServer.SetTempFolderPath(environment.ShadowCopyPath);
            }

            var appDomainRunner = new AppDomainRunner(listener, runOptions);

            try
            {
                appDomainRunner.RunAssembly(new AssemblyPath(environment.AssemblyPath));
            }
            catch (Exception e)
            {
                taskServer.ShowNotification("Unable to run tests: " + e.Message, string.Empty);
                taskServer.TaskException(null, new[] {new TaskException(e)});
            }
        }
    }
}
