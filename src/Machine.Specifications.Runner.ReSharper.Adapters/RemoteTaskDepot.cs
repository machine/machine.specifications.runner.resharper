using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.ReSharper.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public class RemoteTaskDepot
    {
        private readonly Dictionary<string, MspecRemoteTask> tasksByReSharperId = new();

        private readonly Dictionary<string, MspecRemoteTask> behaviorTasksById = new();

        private readonly HashSet<MspecRemoteTask> reportableTasks = new();

        private readonly HashSet<string> contexts = new();

        public RemoteTaskDepot(RemoteTask[] tasks)
        {
            foreach (var task in tasks.OfType<MspecRemoteTask>())
            {
                Add(task);
            }

            foreach (var task in tasks.OfType<MspecBehaviorSpecificationRemoteTask>())
            {
                var behavior = MspecBehaviorRemoteTask.ToServer(task.ContextTypeName, task.BehaviorFieldName, null);
                var id = MspecReSharperId.Create(behavior);

                if (tasksByReSharperId.TryGetValue(id.Id, out var existingBehavior))
                {
                    behaviorTasksById[task.TestId] = existingBehavior;
                }
                else
                {
                    reportableTasks.Add(behavior);
                    tasksByReSharperId[id.Id] = behavior;
                    behaviorTasksById[task.TestId] = behavior;
                }
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

        public MspecRemoteTask? GetBehavior(MspecReSharperId id)
        {
            behaviorTasksById.TryGetValue(id.Id, out var value);

            return value;
        }

        public void Add(MspecRemoteTask task)
        {
            tasksByReSharperId[task.TestId] = task;

            switch (task)
            {
                case MspecContextRemoteTask context:
                    AddContext(context.ContextTypeName);
                    break;

                case MspecBehaviorRemoteTask behavior:
                    AddContext(behavior.ContextTypeName);
                    break;

                case MspecContextSpecificationRemoteTask specification:
                    AddContext(specification.ContextTypeName);
                    break;

                case MspecBehaviorSpecificationRemoteTask behaviorSpecification:
                    AddContext(behaviorSpecification.ContextTypeName);
                    break;
            }
        }

        public IEnumerable<string> GetContextNames()
        {
            return contexts;
        }

        public IEnumerable<RemoteTask> GetTasks()
        {
            return tasksByReSharperId.Values;
        }

        public IEnumerable<RemoteTask> GetReportableTasks()
        {
            return reportableTasks;
        }

        private void AddContext(string? contextName)
        {
            if (!string.IsNullOrEmpty(contextName))
            {
                contexts.Add(contextName!);
            }
        }
    }
}
