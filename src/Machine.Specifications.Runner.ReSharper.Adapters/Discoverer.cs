using System;
using System.Linq;
using System.Threading;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public class Discoverer
    {
        private readonly TestRunRequest request;

        private readonly ITestDiscoverySink sink;

        private readonly RemoteTaskDepot depot;

        private readonly CancellationToken token;

        private readonly ILogger logger = Logger.GetLogger<Discoverer>();

        public Discoverer(TestRunRequest request, ITestDiscoverySink sink, RemoteTaskDepot depot, CancellationToken token)
        {
            this.request = request;
            this.sink = sink;
            this.depot = depot;
            this.token = token;
        }

        public void Discover()
        {
            logger.Info($"Discovering tests from {request.Container.Location}");

            try
            {
                var source = request.Selection
                    .Select(GetRemoteTask)
                    .Where(task => task != null)
                    .ToArray();

                if (source.Any())
                {
                    logger.Debug("Sending discovery results to server");

                    sink.TestsDiscovered(source);
                }
            }
            catch (OperationCanceledException)
            {
                logger.Info("Discovery was aborted");
            }

            logger.Info("Discovery completed");
        }

        private RemoteTask GetRemoteTask(RemoteTask element)
        {
            token.ThrowIfCancellationRequested();

            var task = RemoteTaskBuilder.GetRemoteTask(element);
            var id = MspecReSharperId.Create(task);

            if (depot[id] == null)
            {
                depot.Add(task);
            }

            return task;
        }
    }
}
