namespace Machine.Specifications.Runner.ReSharper.Adapters.Elements
{
    public interface IMspecElement
    {
        string Id { get; }

        string AggregateId { get; }

        string? IgnoreReason { get; }
    }
}
