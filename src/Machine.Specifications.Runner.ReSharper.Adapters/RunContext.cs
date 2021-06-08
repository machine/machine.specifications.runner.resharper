using System.Collections.Concurrent;
using System.Collections.Generic;
using JetBrains.ReSharper.TestRunner.Abstractions;
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

        public TaskWrapper GetTask(Utility.ContextInfo context)
        {
            var key = new MspecReSharperId(context);

            return contexts.GetOrAdd(key, x => new TaskWrapper(depot[x], sink));
        }

        public TaskWrapper GetTask(Utility.ContextInfo? context, Utility.SpecificationInfo specification)
        {
            if (context == null)
            {
                return new TaskWrapper(null, sink);
            }

            var key = new MspecReSharperId(context, specification);

            if (depot[key] == null)
            {
                key = new MspecReSharperId(specification);
            }

            return specifications.GetOrAdd(key, x => new TaskWrapper(depot[x], sink));
        }
    }
}
