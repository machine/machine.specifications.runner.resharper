using System.Xml.Serialization;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Elements
{
    public abstract class TestElement
    {
        [XmlElement("name")]
        public string Name { get; set; }
    }
}
