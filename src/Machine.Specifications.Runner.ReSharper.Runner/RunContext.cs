using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.TaskRunnerFramework;
using Machine.Specifications.Runner.ReSharper.Runner.Tasks;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Runner
{
    public class RunContext
    {
        private readonly Dictionary<string, MspecRunnerTask> remoteTasks = new();

        private readonly List<string> contextNames = new();

        private readonly object sync = new();

        public RunContext(TaskExecutionNode node)
        {
            var tasks = node.Children
                .Flatten(x => x.Children)
                .Select(x => x.RemoteTask)
                .OfType<MspecRunnerTask>()
                .Where(x => !string.IsNullOrEmpty(x.GetKey()));

            foreach (var task in tasks)
            {
                Add(task);
            }
        }

        public IReadOnlyList<string> GetContextNames()
        {
            lock (sync)
            {
                return contextNames;
            }
        }

        public MspecRunnerTask GetContextTask(ContextInfo context)
        {
            var key = context.TypeName;

            return GetRemoteTask(key);
        }

        public MspecRunnerTask GetSpecificationTask(SpecificationInfo specification)
        {
            var key = $"{specification.ContainingType}.{specification.FieldName}";

            return GetRemoteTask(key);
        }

        public MspecRunnerTask GetBehaviorTask(ContextInfo context, SpecificationInfo specification)
        {
            var key = $"{context.TypeName}.{specification.FieldName}";

            return GetRemoteTask(key);
        }

        private void Add(MspecRunnerTask task)
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

        private MspecRunnerTask GetRemoteTask(string key)
        {
            lock (sync)
            {
                remoteTasks.TryGetValue(key, out MspecRunnerTask task);

                return task;
            }
        }
    }
}
