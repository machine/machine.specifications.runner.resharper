using System;
using System.Xml;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Machine.Specifications.Runner.ReSharper.Runner.Tasks
{
    [Serializable]
    public class MspecContextSpecificationRunnerTask : MspecRunnerTask, IEquatable<MspecContextSpecificationRunnerTask>
    {
        public MspecContextSpecificationRunnerTask(string projectId, string contextTypeName, string specificationFieldName, string ignoreReason)
            : base(ignoreReason)
        {
            ProjectId = projectId;
            ContextTypeName = contextTypeName;
            SpecificationFieldName = specificationFieldName;
        }

        public MspecContextSpecificationRunnerTask(XmlElement element)
            : base(element)
        {
            ProjectId = GetXmlAttribute(element, nameof(ProjectId));
            ContextTypeName = GetXmlAttribute(element, nameof(ContextTypeName));
            SpecificationFieldName = GetXmlAttribute(element, nameof(SpecificationFieldName));
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

        public bool Equals(MspecContextSpecificationRunnerTask other)
        {
            return other != null &&
                   other.ProjectId == ProjectId &&
                   other.ContextTypeName == ContextTypeName &&
                   other.SpecificationFieldName == SpecificationFieldName;
        }

        public override bool Equals(RemoteTask other)
        {
            return Equals(other as MspecContextSpecificationRunnerTask);
        }

        public override bool Equals(object other)
        {
            return Equals(other as MspecContextSpecificationRunnerTask);
        }

        public override int GetHashCode()
        {
            return HashCode
                .Of(ProjectId)
                .And(ContextTypeName)
                .And(SpecificationFieldName);
        }

        public override string GetKey()
        {
            return $"{ContextTypeName}.{SpecificationFieldName}";
        }
    }
}
