using System.Linq;
using System.Threading;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.ReSharper.Adapters.Listeners;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters.Execution;

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
        var assembly = request.Container.Location;

        var testsToRun = context.GetTestsToRun().ToArray();

        var contexts = testsToRun
            .Select(x => x.Context.TypeName)
            .Distinct();

        var cache = new ElementCache(testsToRun);
        var tracker = new RunTracker(testsToRun);

        var listener = new TestExecutionListener(context, cache, token);
        var adapter = new ExecutionAdapterRunListener(listener, cache, tracker);
        var loggingListener = new LoggingRunListener(adapter);
        var machineAdapter = new AdapterListener(loggingListener, assembly);

        var runOptions = RunOptions.Custom.FilterBy(contexts);

        var runner = new AppDomainRunner(machineAdapter, runOptions);
        runner.RunAssembly(new AssemblyPath(request.Container.Location));

        listener.Finished.WaitOne();
    }
}
