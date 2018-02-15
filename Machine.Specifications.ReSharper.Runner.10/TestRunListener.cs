using System;
using System.IO;
using System.Linq;
using JetBrains.ReSharper.TaskRunnerFramework;
using Machine.Specifications.ReSharperRunner.Tasks;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.ReSharperRunner
{
    public class TestRunListener : ISpecificationRunListener
    {
        private readonly IRemoteTaskServer _server;
        private readonly TestContext _context;

        private ContextInfo _currentContext;

        private int _specifications;
        private int _successes;
        private int _errors;

        public TestRunListener(IRemoteTaskServer server, TestContext context)
        {
            _server = server;
            _context = context;
        }

        public void OnAssemblyStart(Runner.Utility.AssemblyInfo assemblyInfo)
        {
            Environment.CurrentDirectory = GetWorkingDirectory(_context.AssemblyTask);

            _server.TaskStarting(_context.AssemblyTask);
        }

        public void OnAssemblyEnd(Runner.Utility.AssemblyInfo assemblyInfo)
        {
            Output(_context.AssemblyTask, assemblyInfo.CapturedOutput);
        }

        public void OnRunStart()
        {
        }

        public void OnRunEnd()
        {
        }

        public void OnContextStart(ContextInfo contextInfo)
        {
            _specifications = 0;
            _errors = 0;
            _successes = 0;

            _currentContext = contextInfo;

            var task = _context.GetContextTask(contextInfo);

            if (task != null)
                _server.TaskStarting(task);
        }

        public void OnContextEnd(ContextInfo contextInfo)
        {
            var result = TaskResult.Inconclusive;

            if (_errors > 0)
                result = TaskResult.Error;
            else if (_specifications == _successes)
                result = TaskResult.Success;

            var task = _context.GetContextTask(contextInfo);
            var message = result == TaskResult.Error ? "One or more tests failed" : string.Empty;

            if (task == null)
            {
                return;
            }

            Output(task, contextInfo.CapturedOutput);

            _server.TaskFinished(task, message, result);
        }

        public void OnSpecificationStart(SpecificationInfo specificationInfo)
        {
            var task = _context.GetBehaviorTask(_currentContext, specificationInfo) ??
                       _context.GetSpecificationTask(specificationInfo);

            if (task == null)
                return;

            _server.TaskStarting(task);

            _specifications += 1;
        }

        public void OnSpecificationEnd(SpecificationInfo specificationInfo, Result result)
        {
            var task = _context.GetBehaviorTask(_currentContext, specificationInfo) ??
                       _context.GetSpecificationTask(specificationInfo);

            if (task == null)
                return;

            Output(task, specificationInfo.CapturedOutput);

            if (result.Status == Status.Failing)
            {
                _errors++;
                _server.TaskException(task, GetExceptions(result.Exception));
                _server.TaskFinished(task, GetExceptionMessage(result.Exception), TaskResult.Exception);
            }
            else if (result.Status == Status.Passing)
            {
                _successes++;
                _server.TaskFinished(task, string.Empty, TaskResult.Success);
            }
            else if (result.Status == Status.NotImplemented)
            {
                Output(task, "Not implemented");
                _server.TaskFinished(task, "Not implemented", TaskResult.Inconclusive);
            }
            else if (result.Status == Status.Ignored)
            {
                Output(task, "Ignored");
                _server.TaskFinished(task, string.Empty, TaskResult.Skipped);
            }
        }

        public void OnFatalError(ExceptionResult exceptionResult)
        {
            _server.TaskOutput(_context.AssemblyTask, "Fatal error: " + exceptionResult.Message, TaskOutputType.STDOUT);
            _server.TaskException(_context.AssemblyTask, GetExceptions(exceptionResult));
            _server.TaskFinished(_context.AssemblyTask, GetExceptionMessage(exceptionResult), TaskResult.Exception);

            _errors += 1;
        }

        private void Output(RemoteTask task, string output)
        {
            if (!string.IsNullOrEmpty(output))
                _server.TaskOutput(task, output, TaskOutputType.STDOUT);
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

            return exception != null ? $"{exception.FullTypeName}: {exception.Message}" : string.Empty;
        }

        private string GetWorkingDirectory(MspecTestAssemblyTask task)
        {
            return Path.GetDirectoryName(task.AssemblyLocation);
        }
    }
}
