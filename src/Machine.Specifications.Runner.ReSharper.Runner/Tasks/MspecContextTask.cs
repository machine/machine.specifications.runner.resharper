using System;
using System.Xml;
using JetBrains.Annotations;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Machine.Specifications.Runner.ReSharper.Runner.Tasks
{
    [Serializable]
    public class MspecContextTask : RemoteTask, IEquatable<MspecContextTask>
    {
        [UsedImplicitly]
        public MspecContextTask(XmlElement element)
            : base(element)
        {
            ProjectId = GetXmlAttribute(element, nameof(ProjectId));
            ContextTypeName = GetXmlAttribute(element, nameof(ContextTypeName));
        }

        public MspecContextTask(string projectId, string contextTypeName)
            : base(MspecTaskRunner.RunnerId)
        {
            ProjectId = projectId;
            ContextTypeName = contextTypeName;
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

        public bool Equals(MspecContextTask other)
        {
            return other != null &&
                   other.ProjectId == ProjectId &&
                   other.ContextTypeName == ContextTypeName;
        }

        public override bool Equals(RemoteTask other)
        {
            return Equals(other as MspecContextTask);
        }

        public override bool Equals(object other)
        {
            return Equals(other as MspecContextTask);
        }

        public override int GetHashCode()
        {
            return HashCode.Of(ProjectId)
                .And(ContextTypeName);
        }

        public string GetId()
        {
            return ContextTypeName;
        }
    }
}
