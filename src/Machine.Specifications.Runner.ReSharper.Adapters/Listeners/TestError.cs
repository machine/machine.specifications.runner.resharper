using JetBrains.ReSharper.TestRunner.Abstractions.Objects;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Listeners
{
    public class TestError
    {
        public TestError(string fullTypeName, ExceptionInfo[] exceptions, string exceptionMessage)
        {
            FullTypeName = fullTypeName;
            Exceptions = exceptions;
            ExceptionMessage = exceptionMessage;
        }

        public string FullTypeName { get; }

        public ExceptionInfo[] Exceptions { get; }

        public string ExceptionMessage { get; }
    }
}
