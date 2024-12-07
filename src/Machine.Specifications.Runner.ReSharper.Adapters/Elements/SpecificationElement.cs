namespace Machine.Specifications.Runner.ReSharper.Adapters.Elements;

public class SpecificationElement(
    IContextElement context,
    string fieldName,
    string? ignoreReason = null,
    IBehaviorElement? behavior = null)
    : ISpecificationElement
{
    public string Id { get; } = behavior != null
        ? $"{context.TypeName}.{behavior.FieldName}.{fieldName}"
        : $"{context.TypeName}.{fieldName}";

    public string AggregateId { get; } = behavior != null
        ? $"{context.TypeName}.{behavior.TypeName}.{fieldName}"
        : $"{context.TypeName}.{fieldName}";

    public string? IgnoreReason { get; } = ignoreReason;

    public IContextElement Context { get; } = context;

    public string FieldName { get; } = fieldName;

    public IBehaviorElement? Behavior { get; } = behavior;
}
