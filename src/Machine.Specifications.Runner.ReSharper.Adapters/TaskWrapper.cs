using System.Threading;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.ReSharper.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public class TaskWrapper
    {
        private readonly MspecRemoteTask task;

        private readonly ITestExecutionSink sink;

        private int started;

        private int finished;

        private TestResult result;

        private string message;

        public TaskWrapper(MspecRemoteTask task, ITestExecutionSink sink)
        {
            this.task = task;
            this.sink = sink;
        }

        public void Starting()
        {
            if (Interlocked.Exchange(ref started, 1) != 0)
            {
                return;
            }

            sink.TestStarting(task);
        }

        public void Output(string output)
        {
            if (string.IsNullOrEmpty(output))
            {
                return;
            }

            sink.TestOutput(task, output, TestOutputType.STDOUT);
        }

        public void Skipped(string reason = null)
        {
            result = TestResult.Ignored;
            message = reason ?? task.IgnoreReason;

            Finished();
        }

        public void Passed()
        {
            result = TestResult.Success;
        }

        public void Failed(ExceptionInfo[] exceptions, string exceptionMessage)
        {
            result = TestResult.Failed;
            message = exceptionMessage;

            sink.TestException(task, exceptions);
        }

        public void Finished(bool childTestsFailed = false)
        {
            if (result == TestResult.Inconclusive)
            {
                result = childTestsFailed
                    ? TestResult.Failed
                    : TestResult.Success;

                message = childTestsFailed
                    ? "One or more child tests failed"
                    : string.Empty;
            }

            if (Interlocked.Exchange(ref finished, 1) != 0)
            {
                return;
            }

            sink.TestFinished(task, message, result);
        }
    }
}
