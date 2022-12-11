namespace Machine.Specifications.Runner.ReSharper.Adapters.Listeners;

public class TestContextInfo
{
    public TestContextInfo(string typeName, string capturedOutput)
    {
        TypeName = typeName;
        CapturedOutput = capturedOutput;
    }

    public string TypeName { get; }

    public string CapturedOutput { get; }
}
