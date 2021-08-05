using System;
using System.Linq;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using LoggingLevel = JetBrains.Diagnostics.LoggingLevel;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    public class TestExecutionSink : ITestExecutionSink
    {
        private readonly IMessageBroker broker;

        private readonly IMessageSink sink;

        private readonly ILogger logger = Logger.GetLogger<TestExecutionSink>();

        public TestExecutionSink(IMessageBroker broker, IMessageSink sink)
        {
            this.broker = broker;
            this.sink = sink;
        }

        public void TestStarting(RemoteTask task)
        {
            try
            {
                sink.Output.Log(LoggingLevel.TRACE, "testing starting in execution sink");

                broker.TestStarting(task);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void TestDuration(RemoteTask task, TimeSpan duration)
        {
            try
            {
                broker.TestDuration(task, duration);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void TestException(RemoteTask task, ExceptionInfo[] exceptions)
        {
            try
            {
                var messages = string.Join(",", exceptions.Select(x => x.Message));

                sink.Output.Log(LoggingLevel.TRACE, "test error: " + messages);

                broker.TestException(task, exceptions);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void TestFinished(RemoteTask task, string message, TestResult result)
        {
            try
            {
                sink.Output.Log(LoggingLevel.TRACE, "test finished: " + message);

                broker.TestFinished(task, message, result);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void TestOutput(RemoteTask task, string text, TestOutputType outputType)
        {
            try
            {
                sink.Output.Log(LoggingLevel.TRACE, "test output: " + text);

                broker.TestOutput(task, text, outputType);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void DynamicTestDiscovered(RemoteTask task)
        {
            try
            {
                broker.DynamicTestDiscovered(task);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
