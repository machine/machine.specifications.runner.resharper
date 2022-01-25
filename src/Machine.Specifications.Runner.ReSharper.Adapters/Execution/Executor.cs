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
            var selection = context.GetSelection().ToArray();

            var contexts = context.GetTestsToRun()
                .Select(x => x.Context.TypeName)
                .Distinct();

            var results = new ResultsContainer(selection);

            var listener = new TestExecutionListener(context, token);
            var adapter = new TestAdapterListener(listener, results);

            var runOptions = RunOptions.Custom.FilterBy(contexts);

            var runner = new AppDomainRunner(adapter, runOptions);
            runner.RunAssembly(new AssemblyPath(request.Container.Location));

            adapter.Finished.WaitOne();
        }
    }
}
