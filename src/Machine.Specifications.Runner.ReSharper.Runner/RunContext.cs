using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.TaskRunnerFramework;
using Machine.Specifications.Runner.ReSharper.Runner.Tasks;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Runner
{
    public class RunContext
    {
        private readonly Dictionary<string, RemoteTask> remoteTasks = new();

        private readonly List<string> contextNames = new();

        private readonly object sync = new();

        public RunContext(TaskExecutionNode node)
        {
            var tasks = node.Children
                .Flatten(x => x.Children)
                .Select(x => x.RemoteTask)
                .OfType<IKeyedTask>();

            foreach (var task in tasks)
            {
                Add(task);
            }
        }

        public void Abort()
        {
        }

        public IReadOnlyList<string> GetContextNames()
        {
            lock (sync)
            {
                return contextNames;
            }
        }

        public RemoteTask GetContextTask(ContextInfo context)
        {
            var key = context.TypeName;

            return GetRemoteTask(key);
        }

        public RemoteTask GetSpecificationTask(SpecificationInfo specification)
        {
            var key = $"{specification.ContainingType}.{specification.FieldName}";

            return GetRemoteTask(key);
        }

        public RemoteTask GetBehaviorTask(ContextInfo context, SpecificationInfo specification)
        {
            var key = $"{context.TypeName}.{specification.FieldName}";

            return GetRemoteTask(key);
        }

        private void Add(IKeyedTask task)
        {
            var key = task.GetKey();
            var remote = task.AsRemoteTask();

            lock (sync)
            {
                remoteTasks[key] = remote;

                if (remote is MspecContextRunnerTask)
                {
                    contextNames.Add(key);
                }
            }
        }

        private RemoteTask GetRemoteTask(string key)
        {
            lock (sync)
            {
                remoteTasks.TryGetValue(key, out RemoteTask task);

                return task;
            }
        }
    }
}
