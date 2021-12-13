namespace Machine.Specifications.Runner.ReSharper.Adapters.Models
{
    public interface IContext : IMspecElement
    {
        string TypeName { get; }

        string Subject { get; }
    }
}
