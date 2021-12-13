namespace Machine.Specifications.Runner.ReSharper.Adapters.Elements
{
    public class Context : TestElement
    {
        public string TypeName { get; set; }

        public string Subject { get; set; }

        public Specification[] Specifications { get; set; }
    }
}
