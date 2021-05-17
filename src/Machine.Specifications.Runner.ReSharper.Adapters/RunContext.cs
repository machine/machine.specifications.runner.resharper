using System.Collections.Concurrent;
using System.Collections.Generic;
using JetBrains.ReSharper.TestRunner.Abstractions;
using Machine.Specifications.Runner.ReSharper.Tasks;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public class RunContext
    {
        private readonly RemoteTaskDepot depot;

        private readonly ITestExecutionSink sink;

        private readonly ConcurrentDictionary<MspecReSharperId, TaskWrapper> contexts = new();

        private readonly ConcurrentDictionary<MspecReSharperId, TaskWrapper> specifications = new();

        public RunContext(RemoteTaskDepot depot, ITestExecutionSink sink)
        {
            this.depot = depot;
            this.sink = sink;
        }

        public IEnumerable<string> GetContextNames()
        {
            return depot.GetContextNames();
        }

        public TaskWrapper GetTask(ContextInfo context)
        {
            var key = new MspecReSharperId(context);

            return contexts.GetOrAdd(key, x =>
            {
                var task = depot[x] ?? CreateTask(context);

                return new TaskWrapper(task, sink);
            });
        }

        public TaskWrapper GetTask(ContextInfo context, SpecificationInfo specification)
        {
            var key = new MspecReSharperId(context, specification);

            if (depot[key] == null)
            {
                key = new MspecReSharperId(specification);
            }

            return specifications.GetOrAdd(key, x =>
            {
                var task = depot[x] ?? CreateTask(context, specification);

                return new TaskWrapper(task, sink);
            });
        }

        private MspecRemoteTask CreateTask(object value)
        {
            return RemoteTaskBuilder.GetRemoteTask(value);
        }

        private MspecRemoteTask CreateTask(object context, object specification)
        {
            return RemoteTaskBuilder.GetRemoteTask(context, specification);
        }
    }
}
