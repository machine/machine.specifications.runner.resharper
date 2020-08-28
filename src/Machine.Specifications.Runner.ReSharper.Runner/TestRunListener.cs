using System;
using System.IO;
using System.Linq;
using JetBrains.ReSharper.TaskRunnerFramework;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Runner
{
    public class TestRunListener : ISpecificationRunListener
    {
        private readonly IRemoteTaskServer server;

        private readonly TestContext context;

        private ContextInfo currentContext;

        private int specifications;

        private int successes;

        private int errors;

        public TestRunListener(IRemoteTaskServer server, TestContext context)
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

        public void OnContextStart(ContextInfo contextInfo)
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

        public void OnContextEnd(ContextInfo contextInfo)
        {
            var result = TaskResult.Inconclusive;

            if (errors > 0)
            {
                result = TaskResult.Error;
            }
            else if (specifications == successes)
            {
                result = TaskResult.Success;
            }

            var task = context.GetContextTask(contextInfo);

            if (task == null)
            {
                return;
            }

            Output(task, contextInfo.CapturedOutput);

            var message = result == TaskResult.Error
                ? "One or more tests failed"
                : string.Empty;

            server.TaskFinished(task, message, result);
        }

        public void OnSpecificationStart(SpecificationInfo specificationInfo)
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

        public void OnSpecificationEnd(SpecificationInfo specificationInfo, Result result)
        {
            var task = context.GetBehaviorTask(currentContext, specificationInfo) ??
                       context.GetSpecificationTask(specificationInfo);

            if (task == null)
                return;

            Output(task, specificationInfo.CapturedOutput);

            if (result.Status == Status.Failing)
            {
                errors++;
                server.TaskException(task, GetExceptions(result.Exception));
                server.TaskFinished(task, GetExceptionMessage(result.Exception), TaskResult.Exception);
            }
            else if (result.Status == Status.Passing)
            {
                successes++;
                server.TaskFinished(task, string.Empty, TaskResult.Success);
            }
            else if (result.Status == Status.NotImplemented)
            {
                Output(task, "Not implemented");
                server.TaskFinished(task, "Not implemented", TaskResult.Inconclusive);
            }
            else if (result.Status == Status.Ignored)
            {
                Output(task, "Ignored");
                server.TaskFinished(task, string.Empty, TaskResult.Skipped);
            }
        }

        public void OnFatalError(ExceptionResult exceptionResult)
        {
            server.TaskOutput(null, "Fatal error: " + exceptionResult.Message, TaskOutputType.STDOUT);
            server.TaskException(null, GetExceptions(exceptionResult));
            server.TaskFinished(null, GetExceptionMessage(exceptionResult), TaskResult.Exception);

            errors += 1;
        }

        private void Output(RemoteTask task, string output)
        {
            if (!string.IsNullOrEmpty(output))
            {
                server.TaskOutput(task, output, TaskOutputType.STDOUT);
            }
        }

        private TaskException[] GetExceptions(ExceptionResult result)
        {
            return result.Flatten()
                .Select(x => new TaskException(x.FullTypeName, x.Message, x.StackTrace))
                .ToArray();
        }

        private string GetExceptionMessage(ExceptionResult result)
        {
            var exception = result.Flatten().FirstOrDefault();

            return exception != null
                ? $"{exception.FullTypeName}: {exception.Message}"
                : string.Empty;
        }

        private string GetWorkingDirectory(string assemblyLocation)
        {
            return Path.GetDirectoryName(assemblyLocation);
        }
    }
}
