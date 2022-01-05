namespace Machine.Specifications.Runner.ReSharper.Adapters.Discovery.Elements
{
    public class Specification : TestElement
    {
        public Context Context { get; set; }

        public string ContainingType { get; set; }

        public string FieldName { get; set; }

        public Specification SpecificationField { get; set; }
    }
}
