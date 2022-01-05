namespace Machine.Specifications.Runner.ReSharper.Adapters.Discovery.Elements
{
    public class Context : TestElement
    {
        public string TypeName { get; set; } = null!;

        public string Subject { get; set; } = null!;

        public Specification[] Specifications { get; set; } = null!;
    }
}
