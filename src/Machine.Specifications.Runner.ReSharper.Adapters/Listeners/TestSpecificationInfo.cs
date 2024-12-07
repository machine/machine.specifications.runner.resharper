namespace Machine.Specifications.Runner.ReSharper.Adapters.Listeners;

public class TestSpecificationInfo(string containingType, string fieldName, string capturedOutput)
{
    public string ContainingType { get; } = containingType;

    public string FieldName { get; } = fieldName;

    public string CapturedOutput { get; } = capturedOutput;
}
