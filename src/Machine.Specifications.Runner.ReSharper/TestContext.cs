using System.Collections.Generic;
using JetBrains.ReSharper.TaskRunnerFramework;
using Machine.Specifications.Runner.ReSharper.Tasks;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper
{
    public class TestContext
    {
        private readonly Dictionary<string, RemoteTask> remoteTasks = new Dictionary<string, RemoteTask>();

        private readonly List<string> contextNames = new List<string>();

        public TestContext(MspecTestAssemblyTask assemblyTask)
        {
            AssemblyTask = assemblyTask;
        }

        public MspecTestAssemblyTask AssemblyTask { get; }

        public void Add(MspecTestBehaviorTask task)
        {
            var key = $"{task.ContextTypeName}.{task.SpecificationFieldName}";

            remoteTasks[key] = task;
        }

        public void Add(MspecTestContextTask task)
        {
            var key = task.ContextTypeName;

            remoteTasks[key] = task;
            contextNames.Add(key);
        }

        public void Add(MspecTestSpecificationTask task)
        {
            var key = $"{task.ContextTypeName}.{task.SpecificationFieldName}";

            remoteTasks[key] = task;
        }

        public IEnumerable<string> GetContextNames()
        {
            return contextNames;
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

        private RemoteTask GetRemoteTask(string key)
        {
            remoteTasks.TryGetValue(key, out RemoteTask task);

            return task;
        }
    }
}
