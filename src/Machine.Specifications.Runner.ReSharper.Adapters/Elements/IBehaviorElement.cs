namespace Machine.Specifications.Runner.ReSharper.Adapters.Elements;

public interface IBehaviorElement : IMspecElement
{
    IContextElement Context { get; }

    string TypeName { get; }

    string FieldName { get; }
}
