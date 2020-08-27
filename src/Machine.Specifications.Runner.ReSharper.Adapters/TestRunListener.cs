using System;
using System.IO;
using System.Linq;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.ReSharper.Adapters.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public class TestRunListener : Utility.ISpecificationRunListener
    {
        private readonly IServerAdapter server;

        private readonly TestContext context;

        private Utility.ContextInfo currentContext;

        private int specifications;

        private int successes;

        private int errors;

        public TestRunListener(IServerAdapter server, TestContext context)
        {
            this.server = server;
            this.context = context;
        }

        public void OnAssemblyStart(Utility.AssemblyInfo assemblyInfo)
        {
            Environment.CurrentDirectory = GetWorkingDirectory(context.AssemblyLocation);
        }

        public void OnAssemblyEnd(Utility.AssemblyInfo assemblyInfo)
        {
            Output(default, assemblyInfo.CapturedOutput);
        }

        public void OnRunStart()
        {
        }

        public void OnRunEnd()
        {
        }

        public void OnContextStart(Utility.ContextInfo contextInfo)
        {
            specifications = 0;
            errors = 0;
            successes = 0;

            currentContext = contextInfo;

            var task = context.GetContextTask(contextInfo);

            if (task != null)
            {
                server.TaskStarting(task);
            }
        }

        public void OnContextEnd(Utility.ContextInfo contextInfo)
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

            Output(task, contextInfo.CapturedOutput);

            server.TaskFinished(task, message, result);
        }

        public void OnSpecificationStart(Utility.SpecificationInfo specificationInfo)
        {
            var task = context.GetBehaviorTask(currentContext, specificationInfo) ??
                       context.GetSpecificationTask(specificationInfo);

            if (task == null)
            {
                return;
            }

            server.TaskStarting(task);

            specifications += 1;
        }

        public void OnSpecificationEnd(Utility.SpecificationInfo specificationInfo, Utility.Result result)
        {
            var task = context.GetBehaviorTask(currentContext, specificationInfo) ??
                       context.GetSpecificationTask(specificationInfo);

            if (task == null)
            {
                return;
            }

            Output(task, specificationInfo.CapturedOutput);

            if (result.Status == Utility.Status.Failing)
            {
                errors++;
                server.TaskException(task, GetExceptions(result.Exception));
                server.TaskFinished(task, GetExceptionMessage(result.Exception), TestResult.Failed); // Exception?
            }
            else if (result.Status == Utility.Status.Passing)
            {
                successes++;
                server.TaskFinished(task, string.Empty, TestResult.Success);
            }
            else if (result.Status == Utility.Status.NotImplemented)
            {
                Output(task, "Not implemented");
                server.TaskFinished(task, "Not implemented", TestResult.Inconclusive);
            }
            else if (result.Status == Utility.Status.Ignored)
            {
                Output(task, "Ignored");
                server.TaskFinished(task, string.Empty, TestResult.Ignored);
            }
        }

        public void OnFatalError(Utility.ExceptionResult exceptionResult)
        {
            server.TaskOutput(null, "Fatal error: " + exceptionResult.Message, TestOutputType.STDOUT);
            server.TaskException(null, GetExceptions(exceptionResult));
            server.TaskFinished(null, GetExceptionMessage(exceptionResult), TestResult.Failed); // Exception?

            errors += 1;
        }

        private void Output(MspecRemoteTask task, string output)
        {
            if (!string.IsNullOrEmpty(output))
            {
                server.TaskOutput(task, output, TestOutputType.STDOUT);
            }
        }

        private ExceptionInfo[] GetExceptions(Utility.ExceptionResult result)
        {
            return result.Flatten()
                .Select(x => new ExceptionInfo(x.FullTypeName, x.Message, x.StackTrace))
                .ToArray();
        }

        private string GetExceptionMessage(Utility.ExceptionResult result)
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
