using System.Linq;
using System.Threading;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Execution
{
    public class Executor
    {
        private readonly TestRunRequest request;

        private readonly CancellationToken token;

        private readonly RunContext context;

        public Executor(ITestExecutionSink executionSink, TestRunRequest request, RemoteTaskDepot depot, CancellationToken token)
        {
            this.request = request;
            this.token = token;

            context = new RunContext(depot, executionSink);
        }

        public void Execute()
        {
            var testsToRun = context.GetTestsToRun().ToArray();

            var contexts = testsToRun
                .Select(x => x.Context.TypeName)
                .Distinct();

            var cache = new ElementCache(testsToRun);
            var tracker = new RunTracker(testsToRun);

            var listener = new TestExecutionListener(context, token);
            var adapter = new TestRunListener(listener, cache, tracker);

            var runOptions = RunOptions.Custom.FilterBy(contexts);

            var runner = new AppDomainRunner(adapter, runOptions);
            runner.RunAssembly(new AssemblyPath(request.Container.Location));

            adapter.Finished.WaitOne();
        }
    }
}
