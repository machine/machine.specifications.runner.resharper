using JetBrains.ReSharper.TestRunner.Abstractions.Objects;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Listeners;

public class TestError(string fullTypeName, ExceptionInfo[] exceptions, string exceptionMessage)
{
    public string FullTypeName { get; } = fullTypeName;

    public ExceptionInfo[] Exceptions { get; } = exceptions;

    public string ExceptionMessage { get; } = exceptionMessage;
}
