using System;
using System.Xml;
using JetBrains.Annotations;

namespace Machine.Specifications.ReSharperRunner.Tasks
{
    [Serializable]
    public class ContextTask : Task, IEquatable<ContextTask>
    {
        [UsedImplicitly]
        public ContextTask(XmlElement element)
            : base(element)
        {
            ContextTypeName = GetXmlAttribute(element, AttributeNames.ContextTypeName);
        }

        public ContextTask(string providerId, string assemblyLocation, string contextTypeName)
            : base(providerId, assemblyLocation)
        {
            ContextTypeName = contextTypeName;
        }

        public string ContextTypeName { get; }

        public override bool IsMeaningfulTask => true;

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);

            SetXmlAttribute(element, AttributeNames.ContextTypeName, ContextTypeName);
        }

        public bool Equals(ContextTask other)
        {
            return other != null && base.Equals(other) && Equals(ContextTypeName, other.ContextTypeName);
        }

        public override bool Equals(object other)
        {
            return Equals(other as ContextTask);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = base.GetHashCode();
                result = (result * 397) ^ ContextTypeName.GetHashCode();
                return result;
            }
        }
    }
}