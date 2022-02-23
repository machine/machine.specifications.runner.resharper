namespace Machine.Specifications.Runner.ReSharper.Adapters.Elements
{
    public class SpecificationElement : ISpecificationElement
    {
        public SpecificationElement(IContextElement context, string fieldName, string? ignoreReason = null, IBehaviorElement? behavior = null)
        {
            Id = behavior != null
                ? $"{context.TypeName}.{behavior.FieldName}.{fieldName}"
                : $"{context.TypeName}.{fieldName}";
            AggregateId = behavior != null
                ? $"{context.TypeName}.{behavior.TypeName}.{fieldName}"
                : $"{context.TypeName}.{fieldName}";
            IgnoreReason = ignoreReason;
            Context = context;
            FieldName = fieldName;
            Behavior = behavior;
        }

        public string Id { get; }

        public string AggregateId { get; }

        public string? IgnoreReason { get; }

        public IContextElement Context { get; }

        public string FieldName { get; }

        public IBehaviorElement? Behavior { get; }
    }
}
