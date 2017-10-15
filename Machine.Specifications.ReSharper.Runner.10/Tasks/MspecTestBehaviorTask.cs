using System;
using System.Xml;
using JetBrains.Annotations;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Machine.Specifications.ReSharperRunner.Tasks
{
    [Serializable]
    public class MspecTestBehaviorTask : RemoteTask, IEquatable<MspecTestBehaviorTask>
    {
        [UsedImplicitly]
        public MspecTestBehaviorTask(XmlElement element)
            : base(element)
        {
            ProjectId = GetXmlAttribute(element, AttributeNames.ProjectId);
            ContextTypeName = GetXmlAttribute(element, AttributeNames.ContextTypeName);
            BehaviorTypeName = GetXmlAttribute(element, AttributeNames.BehaviorTypeName);
            SpecificationFieldName = GetXmlAttribute(element, AttributeNames.SpecificationFieldName);
            BehaviorFieldName = GetXmlAttribute(element, AttributeNames.BehaviorFieldName);
        }

        public MspecTestBehaviorTask(string projectId, string contextTypeName, string behaviorTypeName, string behaviorFieldName, string behaviorSpecificationFieldName)
            : base(MspecTaskRunner.RunnerId)
        {
            ProjectId = projectId;
            ContextTypeName = contextTypeName;
            SpecificationFieldName = behaviorSpecificationFieldName;
            BehaviorTypeName = behaviorTypeName;
            BehaviorFieldName = behaviorFieldName;
        }

        public string ProjectId { get; }

        public string ContextTypeName { get; }

        public string BehaviorTypeName { get; }

        public string SpecificationFieldName { get; }

        public string BehaviorFieldName { get; }

        public override bool IsMeaningfulTask => true;

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);

            SetXmlAttribute(element, AttributeNames.ProjectId, ProjectId);
            SetXmlAttribute(element, AttributeNames.ContextTypeName, ContextTypeName);
            SetXmlAttribute(element, AttributeNames.SpecificationFieldName, SpecificationFieldName);
            SetXmlAttribute(element, AttributeNames.BehaviorFieldName, BehaviorFieldName);
            SetXmlAttribute(element, AttributeNames.BehaviorTypeName, BehaviorTypeName);
        }

        public bool Equals(MspecTestBehaviorTask other)
        {
            return other != null &&
                   other.ProjectId == ProjectId &&
                   other.ContextTypeName == ContextTypeName &&
                   other.BehaviorFieldName == BehaviorFieldName &&
                   other.BehaviorTypeName == BehaviorTypeName &&
                   other.SpecificationFieldName == SpecificationFieldName;
        }

        public override bool Equals(RemoteTask other)
        {
            return Equals(other as MspecTestBehaviorTask);
        }

        public override bool Equals(object other)
        {
            return Equals(other as MspecTestBehaviorTask);
        }

        public override int GetHashCode()
        {
            return HashCode.Of(ProjectId)
                .And(ContextTypeName)
                .And(BehaviorFieldName)
                .And(BehaviorTypeName)
                .And(SpecificationFieldName);
        }
    }
}
