using System;
using System.Xml;
using JetBrains.Annotations;

namespace Machine.Specifications.ReSharperRunner.Tasks
{
    [Serializable]
    public class ContextSpecificationTask : Task, IEquatable<ContextSpecificationTask>
    {
        [UsedImplicitly]
        public ContextSpecificationTask(XmlElement element)
            : base(element)
        {
            ContextTypeName = GetXmlAttribute(element, AttributeNames.ContextTypeName);
            SpecificationFieldName = GetXmlAttribute(element, AttributeNames.SpecificationFieldName);
        }

        public ContextSpecificationTask(string providerId,
                                        string assemblyLocation,
                                        string contextTypeName,
                                        string specificationFieldName)
            : base(providerId, assemblyLocation)
        {
            ContextTypeName = contextTypeName;
            SpecificationFieldName = specificationFieldName;
        }

        public string ContextTypeName { get; }

        public string SpecificationFieldName { get; }

        public override bool IsMeaningfulTask => true;

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);

            SetXmlAttribute(element, AttributeNames.ContextTypeName, ContextTypeName);
            SetXmlAttribute(element, AttributeNames.SpecificationFieldName, SpecificationFieldName);
        }

        public bool Equals(ContextSpecificationTask other)
        {
            return other != null && 
                base.Equals(other) && 
                Equals(ContextTypeName, other.ContextTypeName) && 
                Equals(SpecificationFieldName, other.SpecificationFieldName);
        }

        public override bool Equals(object other)
        {
            return Equals(other as ContextSpecificationTask);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = base.GetHashCode();
                result = (result * 397) ^ ContextTypeName.GetHashCode();
                result = (result * 397) ^ SpecificationFieldName.GetHashCode();
                return result;
            }
        }
    }
}