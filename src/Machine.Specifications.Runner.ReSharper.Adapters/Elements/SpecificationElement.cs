namespace Machine.Specifications.Runner.ReSharper.Adapters.Elements
{
    public class SpecificationElement : ISpecificationElement
    {
        public SpecificationElement(IContextElement context, string containingType, string fieldName, string name, ISpecificationElement? behaviorSpecification = null)
        {
            Context = context;
            ContainingType = containingType;
            FieldName = fieldName;
            Name = name;
            BehaviorSpecification = behaviorSpecification;
        }

        public IContextElement Context { get; }

        public string ContainingType { get; }

        public string FieldName { get; }

        public ISpecificationElement? BehaviorSpecification { get; }

        public bool IsBehavior => BehaviorSpecification != null;

        public string Name { get; }
    }
}
