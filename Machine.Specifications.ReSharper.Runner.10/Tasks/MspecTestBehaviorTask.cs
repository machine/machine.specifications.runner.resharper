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
            SpecificationFieldNameOnContext = GetXmlAttribute(element, AttributeNames.SpecificationFieldNameOnContext);
        }

        public MspecTestBehaviorTask(string projectId, string contextTypeName, string specificationFieldNameOnContext, string behaviorSpecificationFieldName, string behaviorTypeName)
            : base(MspecTaskRunner.RunnerId)
        {
            ProjectId = projectId;
            ContextTypeName = contextTypeName;
            BehaviorTypeName = behaviorTypeName;
            SpecificationFieldName = behaviorSpecificationFieldName;
            SpecificationFieldNameOnContext = specificationFieldNameOnContext;
        }

        public string ProjectId { get; }

        public string ContextTypeName { get; }

        public string BehaviorTypeName { get; }

        public string SpecificationFieldName { get; }

        public string SpecificationFieldNameOnContext { get; }

        public override bool IsMeaningfulTask => true;

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);

            SetXmlAttribute(element, AttributeNames.ProjectId, ProjectId);
            SetXmlAttribute(element, AttributeNames.ContextTypeName, ContextTypeName);
            SetXmlAttribute(element, AttributeNames.SpecificationFieldName, SpecificationFieldName);
            SetXmlAttribute(element, AttributeNames.BehaviorTypeName, BehaviorTypeName);
            SetXmlAttribute(element, AttributeNames.SpecificationFieldNameOnContext, SpecificationFieldNameOnContext);
        }

        public bool Equals(MspecTestBehaviorTask other)
        {
            return other != null &&
                   other.ProjectId == ProjectId &&
                   other.ContextTypeName == ContextTypeName &&
                   other.BehaviorTypeName == BehaviorTypeName &&
                   other.SpecificationFieldName == SpecificationFieldName && 
                   other.SpecificationFieldNameOnContext == SpecificationFieldNameOnContext;
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
            var hashCode = ProjectId != null ? ProjectId.GetHashCode() : 0;
            hashCode = (hashCode * 397) ^ (ContextTypeName != null ? ContextTypeName.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (BehaviorTypeName != null ? BehaviorTypeName.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (SpecificationFieldName != null ? SpecificationFieldName.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (SpecificationFieldNameOnContext != null ? SpecificationFieldNameOnContext.GetHashCode() : 0);

            return hashCode;
        }
    }
}
