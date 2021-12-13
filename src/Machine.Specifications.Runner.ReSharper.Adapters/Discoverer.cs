﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
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
                var controller = new MspecController();
                var results = controller.Find(request.Container.Location);

                foreach (var element in results)
                {
                    var task = GetRemoteTask(element);
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

        private MspecRemoteTask GetRemoteTask(TestElement element)
        {
            return RemoteTaskBuilder.GetRemoteTask(element);
        }

        private MspecReSharperId? GetParent(TestElement element)
        {
            if (element is Specification specification)
            {
                return MspecReSharperId.Create();
            }

            return null;
        }
    }
}
