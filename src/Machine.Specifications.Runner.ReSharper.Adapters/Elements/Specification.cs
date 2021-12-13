using System.Xml.Serialization;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Elements
{
    public class Specification : TestElement
    {
        [XmlIgnore]
        public Context Context { get; set; }

        [XmlElement("containingtype")]
        public string ContainingType { get; set; }

        [XmlElement("fieldname")]
        public string FieldName { get; set; }

        public bool IsBehavior()
        {
            return ContainingType != Context.TypeName;
        }
    }
}
