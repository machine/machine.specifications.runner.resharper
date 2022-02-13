namespace Machine.Specifications.Runner.ReSharper.Adapters.Elements
{
    public class BehaviorElement : IBehaviorElement
    {
        public BehaviorElement(IContextElement context, string typeName, string fieldName)
        {
            Id = $"{context.TypeName}.{fieldName}";
            AggregateId = $"{context.TypeName}.{typeName}";
            Context = context;
            TypeName = typeName;
            FieldName = fieldName;
        }

        public string Id { get; }

        public string AggregateId { get; }

        public IContextElement Context { get; }

        public string TypeName { get; }

        public string FieldName { get; }
    }
}
