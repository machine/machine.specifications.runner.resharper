﻿using System;
using System.Threading;
using JetBrains.ReSharper.TestRunner.Abstractions;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public class TestRunListener : ISpecificationRunListener
    {
        private readonly RunContext context;

        private readonly string assemblyPath;

        private readonly CancellationToken token;

        private readonly ILogger logger = Logger.GetLogger<TestRunListener>();

        private readonly ManualResetEvent waitEvent = new(false);

        private ContextInfo? currentContext;

        private TaskWrapper? currentTask;

        private int specifications;

        private int successes;

        private int errors;

        public TestRunListener(RunContext context, string assemblyPath, CancellationToken token)
        {
            this.context = context;
            this.assemblyPath = assemblyPath;
            this.token = token;
        }

        public WaitHandle Finished => waitEvent;

        public void OnAssemblyStart(AssemblyInfo assemblyInfo)
        {
            logger.Trace($"OnAssemblyStart: {assemblyInfo.Location}");

            Environment.CurrentDirectory = assemblyPath;
        }

        public void OnAssemblyEnd(AssemblyInfo assemblyInfo)
        {
            logger.Trace($"OnAssemblyEnd: {assemblyInfo.Location}");

            waitEvent.Set();
        }

        public void OnRunStart()
        {
            logger.Trace("OnRunStart");
        }

        public void OnRunEnd()
        {
            logger.Trace("OnRunEnd");
        }

        public void OnContextStart(ContextInfo contextInfo)
        {
            specifications = 0;
            errors = 0;
            successes = 0;

            currentContext = contextInfo;

            logger.Trace($"OnContextStart: {MspecReSharperId.Self(contextInfo)}");

            logger.Catch(() =>
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                currentTask = context.GetTask(contextInfo);

                currentTask.Starting();
            });
        }

        public void OnContextEnd(ContextInfo contextInfo)
        {
            logger.Trace($"OnContextEnd: {MspecReSharperId.Self(contextInfo)}");

            logger.Catch(() =>
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                currentTask = context.GetTask(contextInfo);

                if (successes == specifications && errors == 0)
                {
                    currentTask.Passed();
                }

                currentTask.Output(contextInfo.CapturedOutput);
                currentTask.Finished(errors > 0);
            });
        }

        public void OnSpecificationStart(SpecificationInfo specificationInfo)
        {
            logger.Trace($"OnSpecificationStart: {MspecReSharperId.Self(specificationInfo)}");

            logger.Catch(() =>
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                var task = context.GetTask(currentContext, specificationInfo);

                if (!task.Exists)
                {
                    return;
                }

                currentTask = task;
                currentTask.Starting();
            });
        }

        public void OnSpecificationEnd(SpecificationInfo specificationInfo, Result result)
        {
            logger.Trace($"OnSpecificationEnd: {MspecReSharperId.Self(specificationInfo)}");

            logger.Catch(() =>
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                var task = context.GetTask(currentContext, specificationInfo);

                if (!task.Exists)
                {
                    return;
                }

                currentTask = task;
                currentTask.Output(specificationInfo.CapturedOutput);

                if (result.Status == Status.Failing)
                {
                    errors++;
                    currentTask.Failed(result.Exception.GetExceptions(), result.Exception.GetExceptionMessage());
                    currentTask.Finished();
                }
                else if (result.Status == Status.Passing)
                {
                    successes++;
                    currentTask.Passed();
                    currentTask.Finished();
                }
                else if (result.Status == Status.NotImplemented)
                {
                    currentTask.Skipped("Not implemented");
                }
                else if (result.Status == Status.Ignored)
                {
                    currentTask.Skipped();
                }
            });
        }

        public void OnFatalError(ExceptionResult exceptionResult)
        {
            if (currentTask != null)
            {
                logger.Trace($"OnFatalError: {exceptionResult.FullTypeName}");

                logger.Catch(() =>
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    currentTask.Output($"Fatal error: {exceptionResult.Message}");
                    currentTask.Failed(exceptionResult.GetExceptions(), exceptionResult.GetExceptionMessage());
                    currentTask.Finished(true);
                });
            }

            errors++;
        }
    }
}
