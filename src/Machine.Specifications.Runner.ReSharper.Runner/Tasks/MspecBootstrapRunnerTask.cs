using System;
using System.Xml;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Machine.Specifications.Runner.ReSharper.Runner.Tasks
{
    [Serializable]
    public class MspecBootstrapRunnerTask : RemoteTask, IEquatable<MspecBootstrapRunnerTask>
    {
        public MspecBootstrapRunnerTask(string projectId)
            : base(MspecTaskRunner.RunnerId)
        {
            ProjectId = projectId;
        }

        public MspecBootstrapRunnerTask(XmlElement element)
            : base(element)
        {
            ProjectId = GetXmlAttribute(element, nameof(ProjectId));
        }

        public string ProjectId { get; }

        public override bool IsMeaningfulTask => false;

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);

            SetXmlAttribute(element, nameof(ProjectId), ProjectId);
        }

        public bool Equals(MspecBootstrapRunnerTask? other)
        {
            return other != null && other.ProjectId == ProjectId;
        }

        public override bool Equals(RemoteTask other)
        {
            return Equals(other as MspecBootstrapRunnerTask);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MspecBootstrapRunnerTask);
        }

        public override int GetHashCode()
        {
            return HashCode.Of(ProjectId);
        }
    }
}
