using System;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;

namespace Machine.Specifications.Runner.ReSharper.Tests.TestFramework.Execution;

public class TestExecutionSink(IMessageBroker broker) : ITestExecutionSink
{
    private readonly ILogger logger = Logger.GetLogger<TestExecutionSink>();

    public void TestStarting(RemoteTask task)
    {
        try
        {
            broker.TestStarting(task).Wait();
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
            broker.TestException(task, exceptions).Wait();
        }
        catch (Exception ex)
        {
            logger.Error(ex);
        }
    }

    public void TestFinished(RemoteTask task, TestOutcome outcome, string message, TimeSpan? duration = null)
    {
        try
        {
            broker.TestFinished(task, outcome, message, duration).Wait();
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
            broker.TestOutput(task, text, outputType).Wait();
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
            broker.DynamicTestDiscovered(task).Wait();
        }
        catch (Exception ex)
        {
            logger.Error(ex);
        }
    }
}
