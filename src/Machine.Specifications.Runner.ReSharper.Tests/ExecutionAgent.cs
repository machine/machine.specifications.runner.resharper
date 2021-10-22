using System.Threading;
using System.Threading.Tasks;
using JetBrains.Application.Components;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using JetBrains.ReSharper.UnitTestFramework.Execution.TestRunner;
using JetBrains.Util;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    public class ExecutionAgent : ITestRunnerExecutionAgent
    {
        public ExecutionAgent(ITestRunnerExecutionContext context, IMessageBroker messageBroker)
        {
            Context = context;
            MessageBroker = messageBroker;
        }

        public Task<int> Shutdown()
        {
            return Task.FromResult(0);
        }

        public string Id { get; } = string.Empty;

        public Lifetime Lifetime { get; } = Lifetime.Eternal;

        public IPreparedProcess Process { get; } = new EmptyPreparedProcess();

        public IMessageBroker MessageBroker { get; }

        public object ActivationOptions { get; } = new object();

        public ITestRunnerExecutionContext Context { get; }

        public async Task RunTests(CancellationToken cancelCt)
        {
            cancelCt.Register(() => MessageBroker.Abort());

            var taskDepot = Context.Container.GetComponent<ITestRunnerRemoteTaskDepot>();

            var loader = Context.Adapter.GetTestAdapterLoader(Context);
            var container = Context.Adapter.GetTestContainer(Context);

            var tasks = taskDepot.GetRemoteTasks(Context.Run);

            await MessageBroker.Initialize(new RemoteAgentInitializationRequest(loader)).ConfigureAwait(false);
            await MessageBroker.RunTests(new TestRunRequest(container, tasks)).ConfigureAwait(false);
        }
    }
}
