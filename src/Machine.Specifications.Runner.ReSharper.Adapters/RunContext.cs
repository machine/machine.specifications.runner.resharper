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

        private readonly ConcurrentDictionary<string, TaskWrapper> behaviors = new();

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

            return contexts.GetOrAdd(key, x => new TaskWrapper(depot[x], sink));
        }

        public TaskWrapper GetTask(ContextInfo? context, SpecificationInfo specification)
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

        public TaskWrapper GetBehaviorTask(ContextInfo? context, SpecificationInfo specification)
        {
            if (context == null)
            {
                return new TaskWrapper(null, sink);
            }

            var key = new MspecReSharperId(context, specification);

            var behavior = depot.GetBehavior(key);

            if (behavior == null)
            {
                return new TaskWrapper(null, sink);
            }

            return behaviors.GetOrAdd(behavior.TestId, x => new TaskWrapper(behavior, sink));
        }
    }
}
