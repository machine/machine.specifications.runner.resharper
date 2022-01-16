namespace Machine.Specifications.Runner.ReSharper.Adapters.Elements
{
    public interface ISpecificationElement : IMspecElement
    {
        IContextElement Context { get; }

        string ContainingType { get; }

        string FieldName { get; }

        ISpecificationElement? BehaviorSpecification { get; }

        bool IsBehavior { get; }
    }
}
