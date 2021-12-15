﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.ReSharper.Adapters.Models;
using Machine.Specifications.Runner.ReSharper.Tasks;

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

            var source = new List<RemoteTask>();

            try
            {
                var controller = new MspecController(token);
                var results = controller.Find(request.Container.Location);

                foreach (var element in results)
                {
                    var task = GetRemoteTask(element);
                    var parent = GetParent(element);

                    source.Add(task);

                    if (parent != null && depot[element] == null && depot[parent] != null)
                    {
                        depot.Add(element, task);
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
            return RemoteTaskBuilder.GetRemoteTask(depot, element);
        }

        private IMspecElement? GetParent(IMspecElement element)
        {
            return element switch
            {
                IContext => null,
                IContextSpecification specification => specification.Context,
                _ => throw new ArgumentOutOfRangeException(nameof(element))
            };
        }
    }
}
