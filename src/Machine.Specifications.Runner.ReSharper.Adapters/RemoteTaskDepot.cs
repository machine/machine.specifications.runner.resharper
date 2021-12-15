using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.ReSharper.Adapters.Models;
using Machine.Specifications.Runner.ReSharper.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public class RemoteTaskDepot
    {
        private readonly Dictionary<string, MspecRemoteTask> tasksByReSharperId = new();

        private readonly List<IContextSpecification> testsToRun = new();

        public RemoteTaskDepot(RemoteTask[] tasks)
        {
            foreach (var task in tasks.OfType<MspecRemoteTask>())
            {
                tasksByReSharperId[task.TestId] = task;
            }
        }

        public MspecRemoteTask? this[IMspecElement element]
        {
            get
            {
                tasksByReSharperId.TryGetValue(MspecReSharperId.Self(element), out var value);

                return value;
            }
        }

        public MspecRemoteTask? this[MspecReSharperId id]
        {
            get
            {
                tasksByReSharperId.TryGetValue(id.Id, out var value);

                return value;
            }
        }

        public void Add(IMspecElement element, MspecRemoteTask task)
        {
            if (element is IContextSpecification { IsBehavior: true })
            {
                tasksByReSharperId[task.TestId] = task;
            }
        }

        public void Bind(IMspecElement element, MspecRemoteTask task)
        {
            if (tasksByReSharperId.ContainsKey(task.TestId) && element is IContextSpecification specification)
            {
                testsToRun.Add(specification);
            }
        }

        public IEnumerable<IContextSpecification> GetTestsToRun()
        {
            return testsToRun;
        }

        public IEnumerable<MspecRemoteTask> GetTasks()
        {
            return tasksByReSharperId.Values;
        }
    }
}
