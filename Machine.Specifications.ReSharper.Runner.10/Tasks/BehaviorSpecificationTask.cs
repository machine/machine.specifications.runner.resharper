using System;
using System.Xml;
using JetBrains.Annotations;

namespace Machine.Specifications.ReSharperRunner.Tasks
{
    [Serializable]
    public class BehaviorSpecificationTask : Task, IEquatable<BehaviorSpecificationTask>
    {
        [UsedImplicitly]
        public BehaviorSpecificationTask(XmlElement element)
            : base(element)
        {
            ContextTypeName = GetXmlAttribute(element, AttributeNames.ContextTypeName);
            BehaviorTypeName = GetXmlAttribute(element, AttributeNames.BehaviorTypeName);
            SpecificationFieldName = GetXmlAttribute(element, AttributeNames.SpecificationFieldName);
            SpecificationFieldNameOnContext = GetXmlAttribute(element, AttributeNames.SpecificationFieldNameOnContext);
        }

        public BehaviorSpecificationTask(string providerId,
                                         string assemblyLocation,
                                         string contextTypeName,
                                         string specificationFieldNameOnContext,
                                         string behaviorSpecificationFieldName,
                                         string behaviorTypeName)
            : base(providerId, assemblyLocation)
        {
            ContextTypeName = contextTypeName;
            BehaviorTypeName = behaviorTypeName;
            SpecificationFieldName = behaviorSpecificationFieldName;
            SpecificationFieldNameOnContext = specificationFieldNameOnContext;
        }

        public string ContextTypeName { get; }

        public string BehaviorTypeName { get; }

        public string SpecificationFieldName { get; }

        string SpecificationFieldNameOnContext { get; }

        public override bool IsMeaningfulTask => true;

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);

            SetXmlAttribute(element, AttributeNames.ContextTypeName, ContextTypeName);
            SetXmlAttribute(element, AttributeNames.SpecificationFieldName, SpecificationFieldName);
            SetXmlAttribute(element, AttributeNames.BehaviorTypeName, BehaviorTypeName);
            SetXmlAttribute(element, AttributeNames.SpecificationFieldNameOnContext, SpecificationFieldNameOnContext);
        }

        public bool Equals(BehaviorSpecificationTask other)
        {
            return other != null &&
                base.Equals(other) &&
                Equals(ContextTypeName, other.ContextTypeName) &&
                Equals(BehaviorTypeName, other.BehaviorTypeName) &&
                Equals(SpecificationFieldName, other.SpecificationFieldName) &&
                Equals(SpecificationFieldNameOnContext, other.SpecificationFieldNameOnContext);
        }

        public override bool Equals(object other)
        {
            return Equals(other as BehaviorSpecificationTask);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = base.GetHashCode();
                result = (result * 397) ^ ContextTypeName.GetHashCode();
                result = (result * 397) ^ BehaviorTypeName.GetHashCode();
                result = (result * 397) ^ SpecificationFieldName.GetHashCode();
                result = (result * 397) ^ SpecificationFieldNameOnContext.GetHashCode();
                return result;
            }
        }
    }
}