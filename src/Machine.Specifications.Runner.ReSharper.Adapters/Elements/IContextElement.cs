namespace Machine.Specifications.Runner.ReSharper.Adapters.Elements;

public interface IContextElement : IMspecElement
{
    string TypeName { get; }

    string Subject { get; }
}
