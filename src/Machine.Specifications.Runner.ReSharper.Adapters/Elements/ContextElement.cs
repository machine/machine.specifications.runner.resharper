namespace Machine.Specifications.Runner.ReSharper.Adapters.Elements;

public class ContextElement(string typeName, string subject, string? ignoreReason) : IContextElement
{
    public string Id { get; } = typeName;

    public string AggregateId => Id;

    public string? IgnoreReason { get; } = ignoreReason;

    public string TypeName { get; } = typeName;

    public string Subject { get; } = subject;
}
