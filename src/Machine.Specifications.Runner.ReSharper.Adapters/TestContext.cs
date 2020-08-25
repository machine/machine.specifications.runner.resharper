using System.Collections.Generic;
using Machine.Specifications.Runner.ReSharper.Adapters.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public class TestContext
    {
        private readonly Dictionary<string, MspecElementRemoteTask> remoteTasks = new Dictionary<string, MspecElementRemoteTask>();

        private readonly List<string> contextNames = new List<string>();

        public TestContext(string assemblyLocation)
        {
            AssemblyLocation = assemblyLocation;
        }

        public string AssemblyLocation { get; }

        public void AddContext(string key, MspecElementRemoteTask task)
        {
            remoteTasks[key] = task;
            contextNames.Add(key);
        }

        public void AddSpecification(string key, MspecElementRemoteTask task)
        {
            remoteTasks[key] = task;
        }

        public IEnumerable<string> GetContextNames()
        {
            return contextNames;
        }

        public MspecElementRemoteTask GetContextTask(Utility.ContextInfo context)
        {
            var key = context.TypeName;

            return GetRemoteTask(key);
        }

        public MspecElementRemoteTask GetSpecificationTask(Utility.SpecificationInfo specification)
        {
            var key = $"{specification.ContainingType}::{specification.FieldName}";

            return GetRemoteTask(key);
        }

        public MspecElementRemoteTask GetBehaviorTask(Utility.ContextInfo context, Utility.SpecificationInfo specification)
        {
            var key = $"{context.TypeName}::{specification.FieldName}";

            return GetRemoteTask(key);
        }

        private MspecElementRemoteTask GetRemoteTask(string key)
        {
            remoteTasks.TryGetValue(key, out MspecElementRemoteTask task);

            return task;
        }
    }
}
