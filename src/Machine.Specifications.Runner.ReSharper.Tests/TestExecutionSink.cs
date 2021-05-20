using System;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    public class TestExecutionSink : ITestExecutionSink
    {
        private readonly IMessageBroker broker;

        private ILogger logger = Logger.GetLogger<TestExecutionSink>();

        public TestExecutionSink(IMessageBroker broker)
        {
            this.broker = broker;
        }

        public void TestStarting(RemoteTask task)
        {
            try
            {
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
