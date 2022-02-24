namespace Machine.Specifications.Runner.ReSharper.Adapters.Listeners
{
    public class TestRunResult
    {
        public TestRunResult(TestStatus status, TestError? exception = null)
        {
            Status = status;
            Exception = exception;
        }

        public TestStatus Status { get; }

        public TestError? Exception { get; }
    }
}
