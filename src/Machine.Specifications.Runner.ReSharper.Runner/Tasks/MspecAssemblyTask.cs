using System;
using System.Xml;
using JetBrains.Annotations;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Machine.Specifications.Runner.ReSharper.Runner.Tasks
{
    [Serializable]
    public class MspecAssemblyTask : RemoteTask, IEquatable<MspecAssemblyTask>
    {
        [UsedImplicitly]
        public MspecAssemblyTask(XmlElement element)
            : base(element)
        {
            AssemblyLocation = GetXmlAttribute(element, AttributeNames.AssemblyLocation);
            ProjectId = GetXmlAttribute(element, AttributeNames.ProjectId);
        }

        public MspecAssemblyTask(string projectId, string assemblyLocation)
            : base(MspecTaskRunner.RunnerId)
        {
            ProjectId = projectId;
            AssemblyLocation = assemblyLocation;
        }

        public string AssemblyLocation { get; }

        public string ProjectId { get; }

        public override bool IsMeaningfulTask => false;

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);

            SetXmlAttribute(element, AttributeNames.AssemblyLocation, AssemblyLocation);
            SetXmlAttribute(element, AttributeNames.ProjectId, ProjectId);
        }

        public override bool Equals(RemoteTask other)
        {
            return Equals(other as MspecAssemblyTask);
        }

        public bool Equals(MspecAssemblyTask other)
        {
            return other != null && other.ProjectId == ProjectId;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MspecAssemblyTask);
        }

        public override int GetHashCode()
        {
            return HashCode.Of(ProjectId);
        }
    }
}
