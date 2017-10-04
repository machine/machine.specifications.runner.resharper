using System.Collections.Generic;
using JetBrains.ReSharper.TaskRunnerFramework;
using Machine.Specifications.ReSharperRunner.Tasks;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.ReSharperRunner
{
    public class TestContext
    {
        private readonly Dictionary<string, RemoteTask> _remoteTasks = new Dictionary<string, RemoteTask>();
        private readonly List<string> _contextNames = new List<string>();

        public TestContext(MspecTestAssemblyTask assemblyTask)
        {
            AssemblyTask = assemblyTask;
        }

        public MspecTestAssemblyTask AssemblyTask { get; }

        public void Add(MspecTestBehaviorTask task)
        {
            var key = $"{task.ContextTypeName}.{task.BehaviorFieldName}.{task.SpecificationFieldName}";

            _remoteTasks[key] = task;
        }

        public void Add(MspecTestContextTask task)
        {
            var key = task.ContextTypeName;

            _remoteTasks[key] = task;
            _contextNames.Add(key);
        }

        public void Add(MspecTestSpecificationTask task)
        {
            var key = $"{task.ContextTypeName}.{task.SpecificationFieldName}";

            _remoteTasks[key] = task;
        }

        public IEnumerable<string> GetContextNames()
        {
            return _contextNames;
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
            var key = $"{context.TypeName}.{specification.ContainingType}.{specification.FieldName}";

            return GetRemoteTask(key);
        }

        private RemoteTask GetRemoteTask(string key)
        {
            _remoteTasks.TryGetValue(key, out RemoteTask task);

            return task;
        }
    }
}
