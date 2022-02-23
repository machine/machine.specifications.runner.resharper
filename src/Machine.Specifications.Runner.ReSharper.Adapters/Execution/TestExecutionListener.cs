using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.ReSharper.Adapters.Listeners;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Execution
{
    public class TestExecutionListener : IExecutionListener
    {
        private readonly RunContext runContext;

        private readonly ElementCache cache;

        private readonly CancellationToken token;

        private readonly HashSet<ISpecificationElement> passed = new();

        private readonly HashSet<ISpecificationElement> failed = new();

        private readonly ManualResetEvent waitEvent = new(false);

        public TestExecutionListener(RunContext runContext, ElementCache cache, CancellationToken token)
        {
            this.runContext = runContext;
            this.cache = cache;
            this.token = token;
        }

        public WaitHandle Finished => waitEvent;

        public void OnAssemblyStart(string assemblyLocation)
        {
            if (string.IsNullOrEmpty(assemblyLocation))
            {
                return;
            }

            var path = Path.GetDirectoryName(assemblyLocation);

            if (!string.IsNullOrEmpty(path))
            {
                Environment.CurrentDirectory = path;
            }
        }

        public void OnAssemblyEnd(string assemblyLocation)
        {
        }

        public void OnRunStart()
        {
        }

        public void OnRunEnd()
        {
            waitEvent.Set();
        }

        public void OnContextStart(IContextElement context)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            runContext.GetTask(context).Starting();
        }

        public void OnContextEnd(IContextElement context, string capturedOutput)
        {
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

            if (string.IsNullOrEmpty(context.IgnoreReason))
            {
                task.Finished(testsFailed);
            }
            else
            {
                task.Skipped(context.IgnoreReason);
            }
        }

        public void OnBehaviorStart(IBehaviorElement behavior)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            runContext.GetTask(behavior).Starting();
        }

        public void OnBehaviorEnd(IBehaviorElement behavior, string capturedOutput)
        {
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

            if (string.IsNullOrEmpty(behavior.IgnoreReason))
            {
                task.Finished(testsFailed);
            }
            else
            {
                task.Skipped(behavior.IgnoreReason);
            }
        }

        public void OnSpecificationStart(ISpecificationElement specification)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            runContext.GetTask(specification).Starting();
        }

        public void OnSpecificationEnd(ISpecificationElement specification, string capturedOutput, TestRunResult runResult)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            var task = runContext.GetTask(specification);

            task.Output(capturedOutput);

            if (runResult.Status == TestStatus.Failing)
            {
                failed.Add(specification);

                task.Failed(runResult.Exception?.Exceptions ?? Array.Empty<ExceptionInfo>(), runResult.Exception?.ExceptionMessage ?? string.Empty);
                task.Finished();
            }
            else if (runResult.Status == TestStatus.Passing)
            {
                passed.Add(specification);

                task.Passed();
                task.Finished();
            }
            else if (runResult.Status == TestStatus.NotImplemented)
            {
                task.Skipped("Not implemented");
            }
            else if (runResult.Status == TestStatus.Ignored)
            {
                task.Skipped();
            }
        }

        public void OnFatalError(TestError? error)
        {
        }
    }
}
