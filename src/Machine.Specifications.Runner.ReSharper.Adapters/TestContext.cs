using System.Collections.Generic;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public class TestContext<TTask>
    {
        private readonly Dictionary<string, TTask> remoteTasks = new Dictionary<string, TTask>();

        private readonly List<string> contextNames = new List<string>();

        public TestContext(string assemblyLocation)
        {
            AssemblyLocation = assemblyLocation;
        }

        public string AssemblyLocation { get; }

        public void AddContext(string key, TTask task)
        {
            remoteTasks[key] = task;
            contextNames.Add(key);
        }

        public void AddSpecification(string key, TTask task)
        {
            remoteTasks[key] = task;
        }

        public IEnumerable<string> GetContextNames()
        {
            return contextNames;
        }

        public TTask GetContextTask(ContextInfo context)
        {
            var key = context.TypeName;

            return GetRemoteTask(key);
        }

        public TTask GetSpecificationTask(SpecificationInfo specification)
        {
            var key = $"{specification.ContainingType}.{specification.FieldName}";

            return GetRemoteTask(key);
        }

        public TTask GetBehaviorTask(ContextInfo context, SpecificationInfo specification)
        {
            var key = $"{context.TypeName}.{specification.FieldName}";

            return GetRemoteTask(key);
        }

        private TTask GetRemoteTask(string key)
        {
            remoteTasks.TryGetValue(key, out TTask task);

            return task;
        }
    }
}
