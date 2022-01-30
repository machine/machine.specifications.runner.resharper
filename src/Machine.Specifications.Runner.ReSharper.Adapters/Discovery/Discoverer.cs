using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.ReSharper.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Discovery
{
    public class Discoverer
    {
        private readonly TestRequest request;

        private readonly IMspecController controller;

        private readonly ITestDiscoverySink sink;

        private readonly RemoteTaskDepot depot;

        private readonly CancellationToken token;

        private readonly ILogger logger = Logger.GetLogger<Discoverer>();

        public Discoverer(TestRequest request, IMspecController controller, ITestDiscoverySink sink, RemoteTaskDepot depot, CancellationToken token)
        {
            this.request = request;
            this.controller = controller;
            this.sink = sink;
            this.depot = depot;
            this.token = token;
        }

        public void Discover()
        {
            logger.Info($"Discovering tests from {request.Container.Location}");

            var source = new List<RemoteTask>();

            try
            {
                var discoverySink = new MspecDiscoverySink(token);

                controller.Find(discoverySink, request.Container.Location);

                foreach (var element in discoverySink.Elements.Result)
                {
                    var task = GetRemoteTask(element);
                    var parent = GetParent(element);

                    source.Add(task);

                    if (parent != null && depot[element] == null && depot[parent] != null)
                    {
                        depot.Add(task);
                    }

                    depot.Bind(element, task);
                }

                if (source.Any())
                {
                    logger.Debug($"Sending {source.Count} discovery results to server");

                    sink.TestsDiscovered(source.ToArray());
                }
            }
            catch (OperationCanceledException)
            {
                logger.Info("Discovery was aborted");
            }

            logger.Info("Discovery completed");
        }

        private MspecRemoteTask GetRemoteTask(IMspecElement element)
        {
            return RemoteTaskBuilder.GetRemoteTask(element);
        }

        private IMspecElement? GetParent(IMspecElement element)
        {
            return element switch
            {
                IContextElement => null,
                IBehaviorElement behavior => behavior.Context,
                ISpecificationElement {Behavior: null} specification => specification.Context,
                ISpecificationElement {Behavior: not null} specification => specification.Behavior,
                _ => throw new ArgumentOutOfRangeException(nameof(element))
            };
        }
    }
}
