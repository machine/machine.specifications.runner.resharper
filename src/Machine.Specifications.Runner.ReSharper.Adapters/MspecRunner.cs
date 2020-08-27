using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.ReSharper.Adapters.Tasks;
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

            var discovered = request.Selection
                .Select(RemoteTaskBuilder.GetRemoteTask)
                .ToArray();

            if (discovered.Any())
            {
                logger.Debug("Sending discovery results to server...");
                discoverySink.TestsDiscovered(discovered);
            }

            var context = GetContext(request);

            var environment = new TestEnvironment(context.AssemblyLocation, request.Container.ShadowCopy != ShadowCopy.None);
            var server = new ExecutionSinkServerAdapter(executionSink);
            var listener = new TestRunListener(server, context);

            var runOptions = Utility.RunOptions.Custom.FilterBy(context.GetContextNames());

            var runner = new AppDomainRunner(listener, runOptions);

            try
            {
                runner.RunAssembly(new AssemblyPath(environment.AssemblyPath));

            }
            catch (Exception e)
            {
                executionSink.TestOutput(null, "Unable to run tests: " + e.Message, TestOutputType.STDERR);
                executionSink.TestException(null, new[] { new ExceptionInfo(e) });
            }

            logger.Info("Execution completed");
        }

        public void AbortRun()
        {
            Debugger.Launch();
            executionToken.Cancel();
        }

        private TestContext GetContext(TestRunRequest request)
        {
            var context = new TestContext(request.Container.Location);

            foreach (var task in request.Selection.OfType<MspecRemoteTask>())
            {
                if (task is MspecContextRemoteTask contextTask)
                {
                    context.AddContext(contextTask.TestId, task);
                }
                else
                {
                    context.AddSpecification(task.TestId, task);
                }
            }

            return context;
        }
    }
}
