using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public class ExecutionSinkServerAdapter : IServerAdapter
    {
        private readonly ITestExecutionSink server;

        public ExecutionSinkServerAdapter(ITestExecutionSink server)
        {
            this.server = server;
        }

        public void TaskStarting(RemoteTask task)
        {
            server.TestStarting(task);
        }

        public void TaskFinished(RemoteTask task, string message, TestResult result)
        {
            server.TestFinished(task, message, result);
        }

        public void TaskException(RemoteTask task, ExceptionInfo[] exceptions)
        {
            server.TestException(task, exceptions);
        }

        public void TaskOutput(RemoteTask task, string message, TestOutputType outputType)
        {
            server.TestOutput(task, message, outputType);
        }
    }
}
