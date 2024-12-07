using System.Collections.Concurrent;
using System.Collections.Generic;
using JetBrains.ReSharper.TestRunner.Abstractions;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.ReSharper.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Execution;

public class RunContext(RemoteTaskDepot depot, ITestExecutionSink sink)
{
    private readonly ConcurrentDictionary<IMspecElement, TaskWrapper> tasks = new();

    private readonly ConcurrentDictionary<string, TaskWrapper> tasksById = new();

    public IEnumerable<ISpecificationElement> GetTestsToRun()
    {
        return depot.GetTestsToRun();
    }

    public TaskWrapper GetTask(IMspecElement element)
    {
        return tasks.GetOrAdd(element, key =>
        {
            var task = depot[MspecReSharperId.Create(key)] ?? CreateTask(element);

            return tasksById.GetOrAdd(task.TestId, _ => new TaskWrapper(task, sink));
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
