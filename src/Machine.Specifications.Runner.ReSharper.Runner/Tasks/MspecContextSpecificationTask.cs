using System;
using System.Xml;
using JetBrains.Annotations;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Machine.Specifications.Runner.ReSharper.Runner.Tasks
{
    [Serializable]
    public class MspecContextSpecificationTask : RemoteTask, IEquatable<MspecContextSpecificationTask>
    {
        [UsedImplicitly]
        public MspecContextSpecificationTask(XmlElement element)
            : base(element)
        {
            ProjectId = GetXmlAttribute(element, nameof(ProjectId));
            ContextTypeName = GetXmlAttribute(element, nameof(ContextTypeName));
            SpecificationFieldName = GetXmlAttribute(element, nameof(SpecificationFieldName));
        }

        public MspecContextSpecificationTask(string projectId, string contextTypeName, string specificationFieldName)
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

            SetXmlAttribute(element, nameof(ProjectId), ProjectId);
            SetXmlAttribute(element, nameof(ContextTypeName), ContextTypeName);
            SetXmlAttribute(element, nameof(SpecificationFieldName), SpecificationFieldName);
        }

        public bool Equals(MspecContextSpecificationTask other)
        {
            return other != null &&
                   other.ProjectId == ProjectId &&
                   other.ContextTypeName == ContextTypeName &&
                   other.SpecificationFieldName == SpecificationFieldName;
        }

        public override bool Equals(RemoteTask other)
        {
            return Equals(other as MspecContextSpecificationTask);
        }

        public override bool Equals(object other)
        {
            return Equals(other as MspecContextSpecificationTask);
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
    }
}
