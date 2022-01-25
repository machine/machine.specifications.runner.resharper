using System;
using System.Threading;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.ReSharper.Adapters.Discovery;
using Machine.Specifications.Runner.ReSharper.Adapters.Execution;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public class MspecRunner : LongLivedMarshalByRefObject, ITestDiscoverer, ITestExecutor
    {
        private readonly ILogger logger = Logger.GetLogger<MspecRunner>();

        private readonly CancellationTokenSource discoveryToken = new();

        private readonly CancellationTokenSource executionToken = new();

        public void DiscoverTests(TestDiscoveryRequest request, ITestDiscoverySink discoverySink)
        {
            logger.Info("Exploration started");

            var depot = new RemoteTaskDepot(Array.Empty<RemoteTask>());

            var discoverer = new Discoverer(request, discoverySink, depot, discoveryToken.Token);
            discoverer.Discover();

            logger.Info("Exploration completed");
        }

        public void AbortDiscovery()
        {
            discoveryToken.Cancel();
        }

        public void RunTests(TestRunRequest request, ITestDiscoverySink discoverySink, ITestExecutionSink executionSink)
        {
            logger.Info("Execution started");

            var depot = new RemoteTaskDepot(request.Selection);

            var discoverer = new Discoverer(request, discoverySink, depot, discoveryToken.Token);
            discoverer.Discover();

            var executor = new Executor(executionSink, request, depot, executionToken.Token);
            executor.Execute();

            logger.Info("Execution completed");
        }

        public void AbortRun()
        {
            executionToken.Cancel();
        }
    }
}
