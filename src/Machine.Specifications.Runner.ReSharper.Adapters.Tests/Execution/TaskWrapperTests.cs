using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.ReSharper.Adapters.Execution;
using Machine.Specifications.Runner.ReSharper.Tasks;
using NSubstitute;
using NUnit.Framework;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Tests.Execution
{
    [TestFixture]
    public class TaskWrapperTests
    {
        [Test]
        public void TaskExists()
        {
            var sink = Substitute.For<ITestExecutionSink>();
            var task = MspecContextRemoteTask.ToServer("ContextType", null, null, null);

            var wrapper = new TaskWrapper(task, sink);

            Assert.True(wrapper.Exists);
        }

        [Test]
        public void StartsOnce()
        {
            var sink = Substitute.For<ITestExecutionSink>();
            var task = MspecContextRemoteTask.ToServer("ContextType", null, null, null);

            var wrapper = new TaskWrapper(task, sink);

            wrapper.Starting();
            wrapper.Starting();

            sink.Received(1).TestStarting(task);
        }

        [Test]
        public void SetsOutput()
        {
            var sink = Substitute.For<ITestExecutionSink>();
            var task = MspecContextRemoteTask.ToServer("ContextType", null, null, null);

            var wrapper = new TaskWrapper(task, sink);

            wrapper.Output("my result");

            sink.Received().TestOutput(task, "my result", TestOutputType.STDOUT);
        }

        [Test]
        public void CallsFinishedOnce()
        {
            var sink = Substitute.For<ITestExecutionSink>();
            var task = MspecContextRemoteTask.ToServer("ContextType", null, null, null);

            var wrapper = new TaskWrapper(task, sink);

            wrapper.Finished();
            wrapper.Finished();

            sink.Received(1).TestFinished(task, Arg.Any<string>(), TestResult.Success);
        }
    }
}
