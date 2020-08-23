using System.Linq;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using RemoteTask = JetBrains.ReSharper.TaskRunnerFramework.RemoteTask;

namespace Machine.Specifications.Runner.ReSharper.Reporting
{
    public class TaskServerAdapter : IServerAdapter<RemoteTask>
    {
        private readonly IRemoteTaskServer server;

        public TaskServerAdapter(IRemoteTaskServer server)
        {
            this.server = server;
        }

        public void TaskStarting(RemoteTask task)
        {
            server.TaskStarting(task);
        }

        public void TaskFinished(RemoteTask task, string message, TestResult result)
        {
            server.TaskFinished(task, message, GetResult(result));
        }

        public void TaskException(RemoteTask task, ExceptionInfo[] exceptions)
        {
            var values = exceptions
                .Select(x => new TaskException(x.Type, x.Message, x.StackTrace))
                .ToArray();

            server.TaskException(task, values);
        }

        public void TaskOutput(RemoteTask task, string message, TestOutputType outputType)
        {
            server.TaskOutput(task, message, GetOutputType(outputType));
        }

        private TaskResult GetResult(TestResult result)
        {
            if (result == TestResult.Inconclusive)
            {
                return TaskResult.Inconclusive;
            }

            if (result == TestResult.Failed)
            {
                return TaskResult.Error;
            }

            return TaskResult.Success;
        }

        private TaskOutputType GetOutputType(TestOutputType type)
        {
            if (type == TestOutputType.STDOUT)
            {
                return TaskOutputType.STDOUT;
            }

            if (type == TestOutputType.STDERR)
            {
                return TaskOutputType.STDERR;
            }

            return TaskOutputType.DEBUGTRACE;
        }
    }
}
