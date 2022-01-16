namespace Machine.Specifications.Runner.ReSharper.Adapters.Elements
{
    public class ContextElement : IContextElement
    {
        public ContextElement(string typeName, string subject, string name)
        {
            TypeName = typeName;
            Subject = subject;
            Name = name;
        }

        public string TypeName { get; }

        public string Subject { get; }

        public string Name { get; }
    }
}
