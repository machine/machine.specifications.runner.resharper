using System;
using System.IO;
using System.Threading;
using JetBrains.ReSharper.TestRunner.Abstractions;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Execution
{
    public class TestRunListener : ISpecificationRunListener
    {
        private readonly RunContext context;

        private readonly CancellationToken token;

        private readonly ManualResetEvent waitEvent = new(false);

        private readonly ILogger logger = Logger.GetLogger<TestRunListener>();

        private ContextInfo? currentContext;

        private TaskWrapper? currentTask;

        private int specifications;

        private int successes;

        private int errors;

        public WaitHandle Finished => waitEvent;

        public TestRunListener(RunContext context, CancellationToken token)
        {
            this.context = context;
            this.token = token;
        }

        public void OnAssemblyStart(AssemblyInfo assemblyInfo)
        {
            logger.Trace($"OnAssemblyStart: {assemblyInfo.Location}");

            Environment.CurrentDirectory = Path.GetDirectoryName(assemblyInfo.Location);
        }

        public void OnAssemblyEnd(AssemblyInfo assemblyInfo)
        {
            logger.Trace($"OnAssemblyEnd: {assemblyInfo.Location}");
        }

        public void OnRunStart()
        {
            logger.Trace("OnRunStart");
        }

        public void OnRunEnd()
        {
            logger.Trace("OnRunEnd");

            waitEvent.Set();
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

                //currentTask = context.GetTask(contextInfo);

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

                //currentTask = context.GetTask(contextInfo);

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
            specifications++;

            logger.Trace($"OnSpecificationStart: {MspecReSharperId.Self(currentContext!, specificationInfo)}");

            logger.Catch(() =>
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                //var task = context.GetTask(currentContext!, specificationInfo);

                //if (!task.Exists)
                //{
                //    return;
                //}

                //currentTask = task;
                //currentTask.Starting();
            });
        }

        public void OnSpecificationEnd(SpecificationInfo specificationInfo, Result result)
        {
            logger.Trace($"OnSpecificationEnd: {MspecReSharperId.Self(currentContext!, specificationInfo)}");

            logger.Catch(() =>
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                //var task = context.GetTask(currentContext!, specificationInfo);

                //if (!task.Exists)
                //{
                //    return;
                //}

                //currentTask = task;
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
            logger.Trace($"OnFatalError: {exceptionResult.FullTypeName}");

            if (currentTask != null)
            {
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
