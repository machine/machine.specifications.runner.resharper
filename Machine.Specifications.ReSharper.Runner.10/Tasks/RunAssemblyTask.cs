using System;
using System.Xml;
using JetBrains.Annotations;

namespace Machine.Specifications.ReSharperRunner.Tasks
{
    [Serializable]
    public class RunAssemblyTask : Task, IEquatable<RunAssemblyTask>
    {
        [UsedImplicitly]
        public RunAssemblyTask(XmlElement element)
            : base(element)
        {
        }

        public RunAssemblyTask(string providerId, string assemblyLocation)
            : base(providerId, assemblyLocation)
        {
        }

        public override bool IsMeaningfulTask => false;

        public bool Equals(RunAssemblyTask other)
        {
            return other != null && base.Equals(other);
        }
    }
}