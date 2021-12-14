using System.Collections.Concurrent;
using System.Collections.Generic;
using JetBrains.ReSharper.TestRunner.Abstractions;
using Machine.Specifications.Runner.ReSharper.Adapters.Models;
using Machine.Specifications.Runner.ReSharper.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public class RunContext
    {
        private readonly RemoteTaskDepot depot;

        private readonly ITestExecutionSink sink;

        private readonly ConcurrentDictionary<MspecReSharperId, TaskWrapper> contexts = new();

        private readonly ConcurrentDictionary<MspecReSharperId, TaskWrapper> specifications = new();

        private readonly HashSet<MspecRemoteTask> handledTasks = new(MspecRemoteTaskComparer.Default);

        private readonly TaskWrapper empty;

        public RunContext(RemoteTaskDepot depot, ITestExecutionSink sink)
        {
            this.depot = depot;
            this.sink = sink;

            empty = new TaskWrapper(null, sink);
        }

        public IEnumerable<IContextSpecification> GetTestsToRun()
        {
            return depot.GetTestsToRun();
        }

        public TaskWrapper GetTask(IContext context)
        {
            var key = new MspecReSharperId(context);

            return contexts.GetOrAdd(key, x => new TaskWrapper(depot[x], sink));
        }

        public TaskWrapper GetTask(IContextSpecification specification)
        {
            var key = new MspecReSharperId(specification);

            return specifications.GetOrAdd(key, x =>
            {
                var task = depot[x];

                if (task == null && specification.IsBehavior)
                {
                    task = CreateTask(specification);
                }

                return new TaskWrapper(task, sink);
            });
        }

        private MspecRemoteTask CreateTask(IMspecElement element)
        {
            var task = RemoteTaskBuilder.GetRemoteTask(element);

            sink.DynamicTestDiscovered(task);

            depot.Add(element, task);

            return task;
        }
    }
}
