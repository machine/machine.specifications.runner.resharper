using System.Collections.Concurrent;
using System.Collections.Generic;
using JetBrains.ReSharper.TestRunner.Abstractions;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.ReSharper.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Execution
{
    public class RunContext
    {
        private readonly RemoteTaskDepot depot;

        private readonly ITestExecutionSink sink;

        private readonly ConcurrentDictionary<IContextElement, TaskWrapper> contextTasks = new();

        private readonly ConcurrentDictionary<ISpecificationElement, TaskWrapper> specificationTasks = new();

        private readonly ConcurrentDictionary<IBehaviorElement, TaskWrapper> behaviorTasks = new();

        private readonly ConcurrentDictionary<string, TaskWrapper> tasksById = new();

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

        public TaskWrapper GetTask(IContextElement context)
        {
            return contextTasks.GetOrAdd(context, key =>
            {
                var task = depot[new MspecReSharperId(key)] ?? CreateTask(context);

                return tasksById.GetOrAdd(task.TestId, y => new TaskWrapper(task, sink));
            });
        }

        public TaskWrapper GetTask(ISpecificationElement specification)
        {
            return specificationTasks.GetOrAdd(specification, key =>
            {
                var task = depot[new MspecReSharperId(key)] ?? CreateTask(specification);

                return tasksById.GetOrAdd(task.TestId, y => new TaskWrapper(task, sink));
            });
        }

        public TaskWrapper GetTask(IBehaviorElement behavior)
        {
            return behaviorTasks.GetOrAdd(behavior, key =>
            {
                var task = depot[new MspecReSharperId(key)] ?? CreateTask(behavior);

                return tasksById.GetOrAdd(task.TestId, y => new TaskWrapper(task, sink));
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
