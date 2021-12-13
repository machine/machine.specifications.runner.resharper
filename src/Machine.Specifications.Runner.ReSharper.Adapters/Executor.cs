using System.IO;
using System.Linq;
using System.Threading;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters
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
            var assemblyPath = Path.GetDirectoryName(request.Container.Location);

            var contexts = context.GetTestsToRun()
                .Select(x => x.Context.TypeName)
                .Distinct();

            var listener = new TestRunListener(context, assemblyPath!, token);
            var runOptions = RunOptions.Custom.FilterBy(contexts);

            var runner = new AppDomainRunner(listener, runOptions);
            runner.RunAssembly(new AssemblyPath(request.Container.Location));

            listener.Finished.WaitOne();
        }
    }
}
