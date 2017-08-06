using System;
using System.Xml;
using JetBrains.Annotations;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Machine.Specifications.ReSharperRunner.Tasks
{
    [Serializable]
    public class MspecTestSpecificationTask : RemoteTask, IEquatable<MspecTestSpecificationTask>
    {
        [UsedImplicitly]
        public MspecTestSpecificationTask(XmlElement element)
            : base(element)
        {
            ProjectId = GetXmlAttribute(element, AttributeNames.ProjectId);
            ContextTypeName = GetXmlAttribute(element, AttributeNames.ContextTypeName);
            SpecificationFieldName = GetXmlAttribute(element, AttributeNames.SpecificationFieldName);
        }

        public MspecTestSpecificationTask(string projectId, string contextTypeName, string specificationFieldName)
            : base(MspecTaskRunner.RunnerId)
        {
            ProjectId = projectId;
            ContextTypeName = contextTypeName;
            SpecificationFieldName = specificationFieldName;
        }

        public string ProjectId { get; }

        public string ContextTypeName { get; }

        public string SpecificationFieldName { get; }

        public override bool IsMeaningfulTask => true;

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);

            SetXmlAttribute(element, AttributeNames.ProjectId, ProjectId);
            SetXmlAttribute(element, AttributeNames.ContextTypeName, ContextTypeName);
            SetXmlAttribute(element, AttributeNames.SpecificationFieldName, SpecificationFieldName);
        }

        public bool Equals(MspecTestSpecificationTask other)
        {
            return other != null &&
                   other.ProjectId == ProjectId &&
                   other.ContextTypeName == ContextTypeName &&
                   other.SpecificationFieldName == SpecificationFieldName;
        }

        public override bool Equals(RemoteTask other)
        {
            return Equals(other as MspecTestSpecificationTask);
        }

        public override bool Equals(object other)
        {
            return Equals(other as MspecTestSpecificationTask);
        }

        public override int GetHashCode()
        {
            var hashCode = ProjectId != null ? ProjectId.GetHashCode() : 0;
            hashCode = (hashCode * 397) ^ (ContextTypeName != null ? ContextTypeName.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (SpecificationFieldName != null ? SpecificationFieldName.GetHashCode() : 0);

            return hashCode;
        }
    }
}
