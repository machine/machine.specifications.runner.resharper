namespace Machine.Specifications.Runner.ReSharper.Adapters.Listeners;

public class TestContextInfo(string typeName, string capturedOutput)
{
    public string TypeName { get; } = typeName;

    public string CapturedOutput { get; } = capturedOutput;
}
