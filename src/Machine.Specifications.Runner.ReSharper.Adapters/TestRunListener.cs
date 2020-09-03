using System;
using System.IO;
using System.Linq;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.ReSharper.Tasks;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public class TestRunListener : ISpecificationRunListener
    {
        private readonly ITestExecutionSink server;

        private readonly TestContext context;

        private readonly ILogger logger;

        private ContextInfo currentContext;

        private RemoteTask currentTask;

        private int specifications;

        private int successes;

        private int errors;

        public TestRunListener(ITestExecutionSink server, TestContext context, ILogger logger)
        {
            this.server = server;
            this.context = context;
            this.logger = logger;
        }

        public void OnAssemblyStart(AssemblyInfo assemblyInfo)
        {
            Environment.CurrentDirectory = GetWorkingDirectory(context.AssemblyLocation);
        }

        public void OnAssemblyEnd(AssemblyInfo assemblyInfo)
        {
            Output(default, assemblyInfo.CapturedOutput);
        }

        public void OnRunStart()
        {
        }

        public void OnRunEnd()
        {
        }

        public void OnContextStart(ContextInfo contextInfo)
        {
            specifications = 0;
            errors = 0;
            successes = 0;

            currentContext = contextInfo;

            var task = context.GetContextTask(contextInfo);

            if (task != null)
            {
                currentTask = task;

                logger.Catch(() => server.TestStarting(task));
            }
        }

        public void OnContextEnd(ContextInfo contextInfo)
        {
            var result = TestResult.Inconclusive;

            if (errors > 0)
            {
                result = TestResult.Failed;
            }
            else if (specifications == successes)
            {
                result = TestResult.Success;
            }

            var task = context.GetContextTask(contextInfo);
            var message = result == TestResult.Failed ? "One or more tests failed" : string.Empty;

            if (task == null)
            {
                return;
            }

            currentTask = task;

            Output(task, contextInfo.CapturedOutput);

            logger.Catch(() => server.TestFinished(task, message, result));
        }

        public void OnSpecificationStart(SpecificationInfo specificationInfo)
        {
            var task = context.GetBehaviorTask(currentContext, specificationInfo) ??
                       context.GetSpecificationTask(specificationInfo);

            if (task == null)
            {
                return;
            }

            currentTask = task;

            logger.Catch(() => server.TestStarting(task));

            specifications += 1;
        }

        public void OnSpecificationEnd(SpecificationInfo specificationInfo, Result result)
        {
            var task = context.GetBehaviorTask(currentContext, specificationInfo) ??
                       context.GetSpecificationTask(specificationInfo);

            if (task == null)
            {
                return;
            }

            currentTask = task;

            Output(task, specificationInfo.CapturedOutput);

            if (result.Status == Status.Failing)
            {
                errors++;
                logger.Catch(() => server.TestException(task, GetExceptions(result.Exception)));
                logger.Catch(() => server.TestFinished(task, GetExceptionMessage(result.Exception), TestResult.Failed));
            }
            else if (result.Status == Status.Passing)
            {
                successes++;
                logger.Catch(() => server.TestFinished(task, string.Empty, TestResult.Success));
            }
            else if (result.Status == Status.NotImplemented)
            {
                Output(task, "Not implemented");
                logger.Catch(() => server.TestFinished(task, "Not implemented", TestResult.Inconclusive));
            }
            else if (result.Status == Status.Ignored)
            {
                Output(task, "Ignored");
                logger.Catch(() => server.TestFinished(task, string.Empty, TestResult.Ignored));
            }
        }

        public void OnFatalError(ExceptionResult exceptionResult)
        {
            if (currentTask != null)
            {
                logger.Catch(() => server.TestOutput(currentTask, "Fatal error: " + exceptionResult.Message, TestOutputType.STDOUT));
                logger.Catch(() => server.TestException(currentTask, GetExceptions(exceptionResult)));
                logger.Catch(() => server.TestFinished(currentTask, GetExceptionMessage(exceptionResult), TestResult.Failed));
            }

            errors += 1;
        }

        private void Output(MspecRemoteTask task, string output)
        {
            if (!string.IsNullOrEmpty(output))
            {
                logger.Catch(() => server.TestOutput(task, output, TestOutputType.STDOUT));
            }
        }

        private ExceptionInfo[] GetExceptions(ExceptionResult result)
        {
            return result.Flatten()
                .Select(x => new ExceptionInfo(x.FullTypeName, x.Message, x.StackTrace))
                .ToArray();
        }

        private string GetExceptionMessage(ExceptionResult result)
        {
            var exception = result.Flatten().FirstOrDefault();

            return exception != null ? $"{exception.FullTypeName}: {exception.Message}" : string.Empty;
        }

        private string GetWorkingDirectory(string assemblyLocation)
        {
            return Path.GetDirectoryName(assemblyLocation);
        }
    }
}
