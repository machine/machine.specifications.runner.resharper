namespace Machine.Specifications.Runner.ReSharper.Adapters.Listeners;

public class TestRunResult(TestStatus status, TestError? exception = null)
{
    public TestStatus Status { get; } = status;

    public TestError? Exception { get; } = exception;
}
