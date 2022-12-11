namespace Machine.Specifications.Runner.ReSharper.Adapters.Elements;

public class ContextElement : IContextElement
{
    public ContextElement(string typeName, string subject, string? ignoreReason)
    {
        Id = typeName;
        TypeName = typeName;
        Subject = subject;
        IgnoreReason = ignoreReason;
    }

    public string Id { get; }

    public string AggregateId => Id;

    public string? IgnoreReason { get; }

    public string TypeName { get; }

    public string Subject { get; }
}
