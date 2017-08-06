using System;
using System.Xml;
using JetBrains.Annotations;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Machine.Specifications.ReSharperRunner.Tasks
{
    [Serializable]
    public class MspecTestContextTask : RemoteTask, IEquatable<MspecTestContextTask>
    {
        [UsedImplicitly]
        public MspecTestContextTask(XmlElement element)
            : base(element)
        {
            ProjectId = GetXmlAttribute(element, AttributeNames.ProjectId);
            ContextTypeName = GetXmlAttribute(element, AttributeNames.ContextTypeName);
        }

        public MspecTestContextTask(string projectId, string contextTypeName)
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

            SetXmlAttribute(element, AttributeNames.ProjectId, ProjectId);
            SetXmlAttribute(element, AttributeNames.ContextTypeName, ContextTypeName);
        }

        public bool Equals(MspecTestContextTask other)
        {
            return other != null &&
                   other.ProjectId == ProjectId &&
                   other.ContextTypeName == ContextTypeName;
        }

        public override bool Equals(RemoteTask other)
        {
            return Equals(other as MspecTestContextTask);
        }

        public override bool Equals(object other)
        {
            return Equals(other as MspecTestContextTask);
        }

        public override int GetHashCode()
        {
            var hashCode = ProjectId != null ? ProjectId.GetHashCode() : 0;
            hashCode = (hashCode * 397) ^ (ContextTypeName != null ? ContextTypeName.GetHashCode() : 0);

            return hashCode;
        }
    }
}