namespace Machine.Specifications.Runner.ReSharper.Adapters.Models
{
    public interface IContextSpecification : IMspecElement
    {
        IContext Context { get; }

        string ContainingType { get; }

        string FieldName { get; }

        string? ParentFieldName { get; }

        bool IsBehavior { get; }
    }
}
