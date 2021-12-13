namespace Machine.Specifications.Runner.ReSharper.Adapters.Elements
{
    public class Specification : TestElement
    {
        public Context Context { get; set; }

        public string ContainingType { get; set; }

        public string FieldName { get; set; }
    }
}
