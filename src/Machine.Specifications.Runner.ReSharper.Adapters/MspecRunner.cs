using System.Linq;
using System.Threading;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.ReSharper.Tasks;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public class MspecRunner : LongLivedMarshalByRefObject, ITestDiscoverer, ITestExecutor
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
            logger.Info("Exploration started");
            logger.Info("Exploration completed");
        }

        public void AbortDiscovery()
        {
            discoveryToken.Cancel();
        }

        public void RunTests(TestRunRequest request, ITestDiscoverySink discoverySink, ITestExecutionSink executionSink)
        {
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
            var listener = new TestRunListener(executionSink, context, logger);

            var runOptions = RunOptions.Custom.FilterBy(context.GetContextNames());

            var runner = new AppDomainRunner(listener, runOptions);
            runner.RunAssembly(new AssemblyPath(environment.AssemblyPath));

            logger.Info("Execution completed");
        }

        public void AbortRun()
        {
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
