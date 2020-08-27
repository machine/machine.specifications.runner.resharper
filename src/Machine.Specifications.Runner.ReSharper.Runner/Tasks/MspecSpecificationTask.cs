using System;
using System.Xml;
using JetBrains.Annotations;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Machine.Specifications.Runner.ReSharper.Runner.Tasks
{
    [Serializable]
    public class MspecSpecificationTask : RemoteTask, IEquatable<MspecSpecificationTask>
    {
        [UsedImplicitly]
        public MspecSpecificationTask(XmlElement element)
            : base(element)
        {
            ProjectId = GetXmlAttribute(element, AttributeNames.ProjectId);
            ContextTypeName = GetXmlAttribute(element, AttributeNames.ContextTypeName);
            SpecificationFieldName = GetXmlAttribute(element, AttributeNames.SpecificationFieldName);
        }

        public MspecSpecificationTask(string projectId, string contextTypeName, string specificationFieldName)
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

        public bool Equals(MspecSpecificationTask other)
        {
            return other != null &&
                   other.ProjectId == ProjectId &&
                   other.ContextTypeName == ContextTypeName &&
                   other.SpecificationFieldName == SpecificationFieldName;
        }

        public override bool Equals(RemoteTask other)
        {
            return Equals(other as MspecSpecificationTask);
        }

        public override bool Equals(object other)
        {
            return Equals(other as MspecSpecificationTask);
        }

        public override int GetHashCode()
        {
            return HashCode.Of(ProjectId)
                .And(ContextTypeName)
                .And(SpecificationFieldName);
        }

        public string GetId()
        {
            return $"{ContextTypeName}.{SpecificationFieldName}";
        }

        public bool IsContext()
        {
            return false;
        }
    }
}
