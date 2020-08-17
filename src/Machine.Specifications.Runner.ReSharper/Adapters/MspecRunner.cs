using System.Diagnostics;
using System.Threading;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public class MspecRunner : ITestDiscoverer, ITestExecutor
    {
        private readonly ILogger logger;

        private readonly CancellationTokenSource discoveryToken = new CancellationTokenSource();

        private readonly CancellationTokenSource executionToken = new CancellationTokenSource();

        public MspecRunner(ILogger logger)
        {
            this.logger = logger;
        }

        public void DiscoverTests(TestDiscoveryRequest request, ITestDiscoverySink discoverySink)
        {
            Debugger.Launch();
            logger.Info("Exploration started");
            logger.Info("Exploration completed");
        }

        public void AbortDiscovery()
        {
            Debugger.Launch();
            discoveryToken.Cancel();
        }

        public void RunTests(TestRunRequest request, ITestDiscoverySink discoverySink, ITestExecutionSink executionSink)
        {
            Debugger.Launch();
            logger.Info("Execution started");

            request

            var listener = new TestRunListener(taskServer, context);

            var runner = new AppDomainRunner(listener, runOptions);

            try
            {
                appDomainRunner.RunAssembly(new AssemblyPath(environment.AssemblyPath));
            }
            catch (Exception e)
            {
                taskServer.ShowNotification("Unable to run tests: " + e.Message, string.Empty);
                taskServer.TaskException(null, new[] {new TaskException(e)});
            }

            logger.Info("Execution completed");
        }

        public void AbortRun()
        {
            Debugger.Launch();
            executionToken.Cancel();
        }
    }
}
