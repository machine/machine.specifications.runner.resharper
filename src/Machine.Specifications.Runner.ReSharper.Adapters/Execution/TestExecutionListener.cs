using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using JetBrains.ReSharper.TestRunner.Abstractions;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Execution
{
    public class TestExecutionListener : IExecutionListener
    {
        private readonly RunContext runContext;

        private readonly ElementCache cache;

        private readonly CancellationToken token;

        private readonly ILogger logger = Logger.GetLogger<TestExecutionListener>();

        private readonly HashSet<ISpecificationElement> passed = new();

        private readonly HashSet<ISpecificationElement> failed = new();

        public TestExecutionListener(RunContext runContext, ElementCache cache, CancellationToken token)
        {
            this.runContext = runContext;
            this.cache = cache;
            this.token = token;
        }

        public void OnAssemblyStart(string assemblyLocation)
        {
            logger.Trace($"OnAssemblyStart: {assemblyLocation}");

            Environment.CurrentDirectory = Path.GetDirectoryName(assemblyLocation);
        }

        public void OnAssemblyEnd(string assemblyLocation)
        {
            logger.Trace($"OnAssemblyEnd: {assemblyLocation}");
        }

        public void OnRunStart()
        {
            logger.Trace("OnRunStart");
        }

        public void OnRunEnd()
        {
            logger.Trace("OnRunEnd");
        }

        public void OnContextStart(IContextElement context)
        {
            logger.Trace($"OnContextStart: {MspecReSharperId.Self(context)}");

            if (token.IsCancellationRequested)
            {
                return;
            }

            runContext.GetTask(context).Starting();
        }

        public void OnContextEnd(IContextElement context, string capturedOutput)
        {
            logger.Trace($"OnContextEnd: {MspecReSharperId.Self(context)}");

            if (token.IsCancellationRequested)
            {
                return;
            }

            var task = runContext.GetTask(context);
            var tests = cache.GetSpecifications(context).ToArray();

            var testsPassed = tests.All(x => passed.Contains(x));
            var testsFailed = tests.Any(x => failed.Contains(x));

            if (testsPassed)
            {
                task.Passed();
            }

            task.Output(capturedOutput);
            task.Finished(testsFailed);
        }

        public void OnBehaviorStart(IBehaviorElement behavior)
        {
            logger.Trace($"OnBehaviorStart: {MspecReSharperId.Self(behavior)}");

            if (token.IsCancellationRequested)
            {
                return;
            }

            runContext.GetTask(behavior).Starting();
        }

        public void OnBehaviorEnd(IBehaviorElement behavior, string capturedOutput)
        {
            logger.Trace($"OnBehaviorEnd: {MspecReSharperId.Self(behavior)}");

            if (token.IsCancellationRequested)
            {
                return;
            }

            var task = runContext.GetTask(behavior);
            var tests = cache.GetSpecifications(behavior).ToArray();

            var testsPassed = tests.All(x => passed.Contains(x));
            var testsFailed = tests.Any(x => failed.Contains(x));

            if (testsPassed)
            {
                task.Passed();
            }

            task.Output(capturedOutput);
            task.Finished(testsFailed);
        }

        public void OnSpecificationStart(ISpecificationElement specification)
        {
            logger.Trace($"OnSpecificationStart: {MspecReSharperId.Self(specification)}");

            if (token.IsCancellationRequested)
            {
                return;
            }

            runContext.GetTask(specification).Starting();
        }

        public void OnSpecificationEnd(ISpecificationElement specification, string capturedOutput, Result result)
        {
            logger.Trace($"OnSpecificationEnd: {MspecReSharperId.Self(specification)}");

            if (token.IsCancellationRequested)
            {
                return;
            }

            var task = runContext.GetTask(specification);

            task.Output(capturedOutput);

            if (result.Status == Status.Failing)
            {
                failed.Add(specification);

                task.Failed(result.Exception.GetExceptions(), result.Exception.GetExceptionMessage());
                task.Finished();
            }
            else if (result.Status == Status.Passing)
            {
                passed.Add(specification);

                task.Passed();
                task.Finished();
            }
            else if (result.Status == Status.NotImplemented)
            {
                task.Skipped("Not implemented");
            }
            else if (result.Status == Status.Ignored)
            {
                task.Skipped();
            }
        }

        public void OnFatalError(ExceptionResult exceptionResult)
        {
            logger.Trace($"OnFatalError: {exceptionResult.FullTypeName}");

            if (token.IsCancellationRequested)
            {
                return;
            }

            // TODO: ??
        }
    }
}
