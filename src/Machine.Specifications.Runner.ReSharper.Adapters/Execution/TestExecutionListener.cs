using System;
using System.IO;
using System.Threading;
using JetBrains.ReSharper.TestRunner.Abstractions;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Execution
{
    public class TestExecutionListener : IExecutionListener
    {
        private readonly RunContext runContext;

        private readonly CancellationToken token;

        private readonly ILogger logger = Logger.GetLogger<TestExecutionListener>();

        public TestExecutionListener(RunContext runContext, CancellationToken token)
        {
            this.runContext = runContext;
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
        }

        public void OnContextEnd(IContextElement context)
        {
            logger.Trace($"OnContextEnd: {MspecReSharperId.Self(context)}");

            if (token.IsCancellationRequested)
            {
                return;
            }
        }

        public void OnBehaviorStart(IBehaviorElement behavior)
        {
            logger.Trace($"OnBehaviorStart: {MspecReSharperId.Self(behavior)}");

            if (token.IsCancellationRequested)
            {
                return;
            }
        }

        public void OnBehaviorEnd(IBehaviorElement behavior)
        {
            logger.Trace($"OnBehaviorEnd: {MspecReSharperId.Self(behavior)}");

            if (token.IsCancellationRequested)
            {
                return;
            }
        }

        public void OnSpecificationStart(ISpecificationElement specification)
        {
            logger.Trace($"OnSpecificationStart: {MspecReSharperId.Self(specification)}");

            if (token.IsCancellationRequested)
            {
                return;
            }
        }

        public void OnSpecificationEnd(ISpecificationElement specification, Result result)
        {
            logger.Trace($"OnSpecificationEnd: {MspecReSharperId.Self(specification)}");

            if (token.IsCancellationRequested)
            {
                return;
            }
        }

        public void OnFatalError(ExceptionResult exceptionResult)
        {
            logger.Trace($"OnFatalError: {exceptionResult.FullTypeName}");

            if (token.IsCancellationRequested)
            {
                return;
            }
        }
    }
}
