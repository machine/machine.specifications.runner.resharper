using System;
using System.Linq;
using System.Reflection;
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
                foreach (var task in request.Selection)
                {
                    GetRemoteTask(task);
                }

                var source = depot.GetTasks().ToArray();

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

        public void ReportAll()
        {
            Action<string> listener = s =>
            {
                Console.WriteLine(s);
            };

            var assemblyName = AssemblyName.GetAssemblyName(request.Container.Location);
            var assembly = Assembly.Load(assemblyName);

            var controllerType = Type.GetType("Machine.Specifications.Controller.Controller, Machine.Specifications");
            var discoverMember = controllerType.GetMethod("DiscoverSpecs", BindingFlags.Instance | BindingFlags.Public);

            var controller = Activator.CreateInstance(controllerType, listener);
            var results = discoverMember.Invoke(controller, new object[] {assembly});
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
