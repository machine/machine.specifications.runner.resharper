﻿using JetBrains.ReSharper.TestRunner.Abstractions.Objects;

namespace Machine.Specifications.Runner.ReSharper.Reporting
{
    public interface IServerAdapter<in TTask>
    {
        void TaskStarting(TTask task);

        void TaskFinished(TTask task, string message, TestResult result);

        void TaskException(TTask task, ExceptionInfo[] exceptions);

        void TaskOutput(TTask task, string message, TestOutputType outputType);
    }
}
