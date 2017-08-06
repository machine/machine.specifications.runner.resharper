using System;
using JetBrains.ReSharper.TaskRunnerFramework;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.ReSharperRunner
{
    public class TestRunner
    {
        private readonly IRemoteTaskServer _taskServer;

        public TestRunner(IRemoteTaskServer taskServer)
        {
            _taskServer = taskServer;
        }

        public void Run(TestContext context)
        {
            var environment = new TestEnvironment(context.AssemblyTask);
            var listener = new TestRunListener(_taskServer, context);

            var runOptions = RunOptions.Custom.FilterBy(context.GetContextNames());

            if (environment.ShouldShadowCopy)
            {
                runOptions.ShadowCopyTo(environment.ShadowCopyPath);
                _taskServer.SetTempFolderPath(environment.ShadowCopyPath);
            }

            Environment.CurrentDirectory = environment.AssemblyFolder;

            var appDomainRunner = new AppDomainRunner(listener, runOptions);

            try
            {
                appDomainRunner.RunAssembly(new AssemblyPath(environment.AssemblyPath));
            }
            catch (Exception e)
            {
                _taskServer.ShowNotification("Unable to run tests: " + e.Message, string.Empty);
                _taskServer.TaskException(context.AssemblyTask, new[] {new TaskException(e)});
            }
        }
    }
}
