namespace Machine.Specifications.Runner.ReSharper.Adapters.Elements
{
    public interface ISpecificationElement : IMspecElement
    {
        IContextElement Context { get; }

        string FieldName { get; }

        IBehaviorElement? Behavior { get; }
    }
}
