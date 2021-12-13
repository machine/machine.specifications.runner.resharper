using System.Xml.Serialization;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Elements
{
    public class Context : TestElement
    {
        [XmlElement("typename")]
        public string TypeName { get; set; }

        [XmlElement("concern")]
        public string Subject { get; set; }

        [XmlArray("specifications")]
        [XmlArrayItem("specificationinfo")]
        public Specification[] Specifications { get; set; }
    }
}
