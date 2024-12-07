namespace Machine.Specifications.Runner.ReSharper.Adapters.Elements;

public class BehaviorElement(IContextElement context, string typeName, string fieldName, string? ignoreReason)
    : IBehaviorElement
{
    public string Id { get; } = $"{context.TypeName}.{fieldName}";

    public string AggregateId { get; } = $"{context.TypeName}.{typeName}";

    public string? IgnoreReason { get; } = ignoreReason;

    public IContextElement Context { get; } = context;

    public string TypeName { get; } = typeName;

    public string FieldName { get; } = fieldName;
}
