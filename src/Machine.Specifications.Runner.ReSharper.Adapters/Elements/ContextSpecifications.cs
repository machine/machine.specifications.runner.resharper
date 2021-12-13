using System.Xml.Serialization;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Elements
{
    [XmlRoot("contexts")]
    public class ContextSpecifications
    {
        [XmlElement("contextinfo")]
        public Context[] Contexts { get; set; }
    }
}
