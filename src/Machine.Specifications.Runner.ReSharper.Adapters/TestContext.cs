using System.Collections.Generic;
using Machine.Specifications.Runner.ReSharper.Adapters.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public class TestContext
    {
        private readonly Dictionary<string, MspecRemoteTask> remoteTasks = new Dictionary<string, MspecRemoteTask>();

        private readonly List<string> contextNames = new List<string>();

        public TestContext(string assemblyLocation)
        {
            AssemblyLocation = assemblyLocation;
        }

        public string AssemblyLocation { get; }

        public void AddContext(string key, MspecRemoteTask task)
        {
            remoteTasks[key] = task;
            contextNames.Add(key);
        }

        public void AddSpecification(string key, MspecRemoteTask task)
        {
            remoteTasks[key] = task;
        }

        public IEnumerable<string> GetContextNames()
        {
            return contextNames;
        }

        public MspecRemoteTask GetContextTask(Utility.ContextInfo context)
        {
            var key = context.TypeName;

            return GetRemoteTask(key);
        }

        public MspecRemoteTask GetSpecificationTask(Utility.SpecificationInfo specification)
        {
            var key = $"{specification.ContainingType}::{specification.FieldName}";

            return GetRemoteTask(key);
        }

        public MspecRemoteTask GetBehaviorTask(Utility.ContextInfo context, Utility.SpecificationInfo specification)
        {
            var key = $"{context.TypeName}::{specification.FieldName}";

            return GetRemoteTask(key);
        }

        private MspecRemoteTask GetRemoteTask(string key)
        {
            remoteTasks.TryGetValue(key, out MspecRemoteTask task);

            return task;
        }
    }
}
