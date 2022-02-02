namespace Machine.Specifications.Runner.ReSharper.Adapters.Elements
{
    public class ContextElement : IContextElement
    {
        public ContextElement(string typeName, string subject)
        {
            Id = typeName;
            TypeName = typeName;
            Subject = subject;
        }

        public string Id { get; }

        public string GroupId => Id;

        public string TypeName { get; }

        public string Subject { get; }
    }
}
