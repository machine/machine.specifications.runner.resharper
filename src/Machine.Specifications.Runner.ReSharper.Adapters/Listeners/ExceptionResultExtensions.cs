using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Listeners;

internal static class ExceptionResultExtensions
{
    public static TestError? ToTestError(this ExceptionResult? result)
    {
        if (result == null)
        {
            return null;
        }

        return new TestError(result.FullTypeName, result.GetExceptions(), result.GetExceptionMessage());
    }
}
