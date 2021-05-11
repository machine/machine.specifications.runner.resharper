using System;
using System.Xml;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Machine.Specifications.Runner.ReSharper.Runner.Tasks
{
    [Serializable]
    public class MspecAssemblyRunnerTask : RemoteTask, IEquatable<MspecAssemblyRunnerTask>
    {
        public MspecAssemblyRunnerTask(string projectId, string assemblyLocation)
            : base(MspecTaskRunner.RunnerId)
        {
            ProjectId = projectId;
            AssemblyLocation = assemblyLocation;
        }

        public MspecAssemblyRunnerTask(XmlElement element)
            : base(element)
        {
            ProjectId = GetXmlAttribute(element, nameof(ProjectId));
            AssemblyLocation = GetXmlAttribute(element, nameof(AssemblyLocation));
        }

        public string ProjectId { get; }

        public string AssemblyLocation { get; }

        public override bool IsMeaningfulTask => false;

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);

            SetXmlAttribute(element, nameof(ProjectId), ProjectId);
            SetXmlAttribute(element, nameof(AssemblyLocation), AssemblyLocation);
        }

        public bool Equals(MspecAssemblyRunnerTask other)
        {
            return other != null && other.ProjectId == ProjectId;
        }

        public override bool Equals(RemoteTask other)
        {
            return Equals(other as MspecAssemblyRunnerTask);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MspecAssemblyRunnerTask);
        }

        public override int GetHashCode()
        {
            return HashCode.Of(ProjectId);
        }
    }
}
