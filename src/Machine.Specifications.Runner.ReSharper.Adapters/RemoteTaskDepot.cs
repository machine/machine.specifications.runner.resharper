using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.ReSharper.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public class RemoteTaskDepot
    {
        private readonly Dictionary<string, MspecRemoteTask> tasksByReSharperId = new();

        private readonly List<string> contexts = new();

        public RemoteTaskDepot(RemoteTask[] tasks)
        {
            foreach (var task in tasks.OfType<MspecRemoteTask>())
            {
                tasksByReSharperId[task.TestId] = task;

                if (task is MspecContextRemoteTask context)
                {
                    contexts.Add(context.ContextTypeName);
                }
            }
        }

        public MspecRemoteTask this[MspecReSharperId id]
        {
            get
            {
                tasksByReSharperId.TryGetValue(id.Id, out var value);

                return value;
            }
        }

        public void Add(MspecRemoteTask task)
        {
            tasksByReSharperId[task.TestId] = task;

            if (task is MspecContextRemoteTask context)
            {
                contexts.Add(context.ContextTypeName);
            }
        }

        public IEnumerable<string> GetContextNames()
        {
            return contexts;
        }
    }
}
