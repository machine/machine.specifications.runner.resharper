namespace Machine.Specifications.Runner.ReSharper.Adapters.Elements;

public class BehaviorElement : IBehaviorElement
{
    public BehaviorElement(IContextElement context, string typeName, string fieldName, string? ignoreReason)
    {
        Id = $"{context.TypeName}.{fieldName}";
        AggregateId = $"{context.TypeName}.{typeName}";
        Context = context;
        TypeName = typeName;
        FieldName = fieldName;
        IgnoreReason = ignoreReason;
    }

    public string Id { get; }

    public string AggregateId { get; }

    public string? IgnoreReason { get; }

    public IContextElement Context { get; }

    public string TypeName { get; }

    public string FieldName { get; }
}
