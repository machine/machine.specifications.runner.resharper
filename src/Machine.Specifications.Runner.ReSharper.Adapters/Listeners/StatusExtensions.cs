using System;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Listeners;

internal static class StatusExtensions
{
    public static TestStatus ToTestStatus(this Status status)
    {
        return status switch
        {
            Status.Ignored => TestStatus.Ignored,
            Status.Failing => TestStatus.Failing,
            Status.NotImplemented => TestStatus.NotImplemented,
            Status.Passing => TestStatus.Passing,
            _ => throw new ArgumentException()
        };
    }
}
