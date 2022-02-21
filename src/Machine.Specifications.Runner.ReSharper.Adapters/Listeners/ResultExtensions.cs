using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Listeners
{
    internal static class ResultExtensions
    {
        public static TestRunResult ToTestResult(this Result result)
        {
            return new TestRunResult(result.Status.ToTestStatus(), result.Exception.ToTestError());
        }
    }
}
