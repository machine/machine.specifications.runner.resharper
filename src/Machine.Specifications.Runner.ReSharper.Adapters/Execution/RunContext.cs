using System.Collections.Concurrent;
using System.Collections.Generic;
using JetBrains.ReSharper.TestRunner.Abstractions;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.ReSharper.Tasks;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Execution
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

        public IEnumerable<IMspecElement> GetSelection()
        {
            return depot.GetSelection();
        }

        public IEnumerable<ISpecificationElement> GetTestsToRun()
        {
            return depot.GetTestsToRun();
        }

        public TaskWrapper GetTask(ContextInfo context)
        {
            var key = new MspecReSharperId(context);

            return contexts.GetOrAdd(key, x => new TaskWrapper(depot[x], sink));
        }

        public TaskWrapper GetTask(ContextInfo context, SpecificationInfo specification)
        {
            var key = new MspecReSharperId(context, specification);

            return specifications.GetOrAdd(key, x =>
            {
                var isBehavior = context.TypeName != specification.ContainingType;
                var task = depot[x];

                if (task == null && isBehavior)
                {
                    //task = CreateTask(specification);
                }

                return new TaskWrapper(task, sink);
            });
        }

        public TaskWrapper GetTask(IContextElement context)
        {
            var key = new MspecReSharperId(context);

            return contexts.GetOrAdd(key, x => new TaskWrapper(depot[x], sink));
        }

        public TaskWrapper GetTask(ISpecificationElement specification)
        {
            var key = new MspecReSharperId(specification);

            return specifications.GetOrAdd(key, x =>
            {
                var task = depot[x];

                if (task == null && specification.Behavior != null)
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

            depot.Add(task);

            return task;
        }
    }
}
