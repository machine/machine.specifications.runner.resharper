namespace Machine.Specifications.Runner.ReSharper.Adapters.Listeners
{
    public class TestSpecificationInfo
    {
        public TestSpecificationInfo(string containingType, string fieldName, string capturedOutput)
        {
            ContainingType = containingType;
            FieldName = fieldName;
            CapturedOutput = capturedOutput;
        }

        public string ContainingType { get; }

        public string FieldName { get; }

        public string CapturedOutput { get; }
    }
}
