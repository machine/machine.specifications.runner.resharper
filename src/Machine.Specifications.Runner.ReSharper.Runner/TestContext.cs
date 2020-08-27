using System.Collections.Generic;
using JetBrains.ReSharper.TaskRunnerFramework;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Runner
{
    public class TestContext
    {
        private readonly Dictionary<string, RemoteTask> remoteTasks = new Dictionary<string, RemoteTask>();

        private readonly List<string> contextNames = new List<string>();

        public TestContext(string assemblyLocation)
        {
            AssemblyLocation = assemblyLocation;
        }

        public string AssemblyLocation { get; }

        public void AddContext(string key, RemoteTask task)
        {
            remoteTasks[key] = task;
            contextNames.Add(key);
        }

        public void AddSpecification(string key, RemoteTask task)
        {
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
