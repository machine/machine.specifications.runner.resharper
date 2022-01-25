namespace Machine.Specifications.Runner.ReSharper.Adapters.Elements
{
    public class SpecificationElement : ISpecificationElement
    {
        public SpecificationElement(IContextElement context, string fieldName, IBehaviorElement? behavior = null)
        {
            Id = behavior != null
                ? $"{context.TypeName}.{behavior.TypeName}.{fieldName}"
                : $"{context.TypeName}.{fieldName}";
            Context = context;
            FieldName = fieldName;
            Behavior = behavior;
        }

        public string Id { get; }

        public IContextElement Context { get; }

        public string FieldName { get; }

        public IBehaviorElement? Behavior { get; }
    }
}
