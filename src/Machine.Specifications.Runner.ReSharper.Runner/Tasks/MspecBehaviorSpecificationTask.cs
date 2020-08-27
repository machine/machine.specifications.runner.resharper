using System;
using System.Xml;
using JetBrains.Annotations;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Machine.Specifications.Runner.ReSharper.Runner.Tasks
{
    [Serializable]
    public class MspecBehaviorSpecificationTask : RemoteTask, IEquatable<MspecBehaviorSpecificationTask>
    {
        [UsedImplicitly]
        public MspecBehaviorSpecificationTask(XmlElement element)
            : base(element)
        {
            ProjectId = GetXmlAttribute(element, nameof(ProjectId));
            ContextTypeName = GetXmlAttribute(element, nameof(ContextTypeName));
            SpecificationFieldName = GetXmlAttribute(element, nameof(SpecificationFieldName));
            BehaviorFieldName = GetXmlAttribute(element, nameof(BehaviorFieldName));
        }

        public MspecBehaviorSpecificationTask(string projectId, string contextTypeName, string behaviorFieldName, string behaviorSpecificationFieldName)
            : base(MspecTaskRunner.RunnerId)
        {
            ProjectId = projectId;
            ContextTypeName = contextTypeName;
            SpecificationFieldName = behaviorSpecificationFieldName;
            BehaviorFieldName = behaviorFieldName;
        }

        public string ProjectId { get; }

        public string ContextTypeName { get; }

        public string SpecificationFieldName { get; }

        public string BehaviorFieldName { get; }

        public override bool IsMeaningfulTask => true;

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);

            SetXmlAttribute(element, nameof(ProjectId), ProjectId);
            SetXmlAttribute(element, nameof(ContextTypeName), ContextTypeName);
            SetXmlAttribute(element, nameof(SpecificationFieldName), SpecificationFieldName);
            SetXmlAttribute(element, nameof(BehaviorFieldName), BehaviorFieldName);
        }

        public bool Equals(MspecBehaviorSpecificationTask other)
        {
            return other != null &&
                   other.ProjectId == ProjectId &&
                   other.ContextTypeName == ContextTypeName &&
                   other.BehaviorFieldName == BehaviorFieldName &&
                   other.SpecificationFieldName == SpecificationFieldName;
        }

        public override bool Equals(RemoteTask other)
        {
            return Equals(other as MspecBehaviorSpecificationTask);
        }

        public override bool Equals(object other)
        {
            return Equals(other as MspecBehaviorSpecificationTask);
        }

        public override int GetHashCode()
        {
            return HashCode.Of(ProjectId)
                .And(ContextTypeName)
                .And(BehaviorFieldName)
                .And(SpecificationFieldName);
        }

        public string GetId()
        {
            return $"{ContextTypeName}.{SpecificationFieldName}";
        }
    }
}
