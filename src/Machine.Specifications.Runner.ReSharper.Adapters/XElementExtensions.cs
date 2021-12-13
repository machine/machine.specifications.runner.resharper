using System.Xml.Linq;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public static class XElementExtensions
    {
        public static string ReadValue(this XElement element, string field)
        {
            var childElement = element.Element(field);

            return childElement == null
                ? string.Empty
                : childElement.Value;
        }
    }
}
