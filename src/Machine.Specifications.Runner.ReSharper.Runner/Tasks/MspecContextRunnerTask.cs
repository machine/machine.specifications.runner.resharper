using System;
using System.Xml;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Machine.Specifications.Runner.ReSharper.Runner.Tasks
{
    [Serializable]
    public class MspecContextRunnerTask : MspecRunnerTask, IEquatable<MspecContextRunnerTask>
    {
        public MspecContextRunnerTask(string projectId, string contextTypeName, string ignoreReason)
            : base(ignoreReason)
        {
            ProjectId = projectId;
            ContextTypeName = contextTypeName;
        }

        public MspecContextRunnerTask(XmlElement element)
            : base(element)
        {
            ProjectId = GetXmlAttribute(element, nameof(ProjectId));
            ContextTypeName = GetXmlAttribute(element, nameof(ContextTypeName));
        }

        public string ProjectId { get; }

        public string ContextTypeName { get; }

        public override bool IsMeaningfulTask => true;

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);

            SetXmlAttribute(element, nameof(ProjectId), ProjectId);
            SetXmlAttribute(element, nameof(ContextTypeName), ContextTypeName);
        }

        public bool Equals(MspecContextRunnerTask other)
        {
            return other != null &&
                   other.ProjectId == ProjectId &&
                   other.ContextTypeName == ContextTypeName;
        }

        public override bool Equals(RemoteTask other)
        {
            return Equals(other as MspecContextRunnerTask);
        }

        public override bool Equals(object other)
        {
            return Equals(other as MspecContextRunnerTask);
        }

        public override int GetHashCode()
        {
            return HashCode
                .Of(ProjectId)
                .And(ContextTypeName);
        }

        public override string GetKey()
        {
            return ContextTypeName;
        }
    }
}
