using JetBrains.ReSharper.TestRunner.Abstractions.Objects;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public interface IServerAdapter
    {
        void TaskStarting(RemoteTask task);

        void TaskFinished(RemoteTask task, string message, TestResult result);

        void TaskException(RemoteTask task, ExceptionInfo[] exceptions);

        void TaskOutput(RemoteTask task, string message, TestOutputType outputType);
    }
}
