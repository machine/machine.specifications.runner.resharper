using System.Threading;
using System.Threading.Tasks;
using JetBrains.Application.Components;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using JetBrains.ReSharper.UnitTestFramework.TestRunner;
using JetBrains.Util;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    public class MspecExecutionAgent : ITestRunnerExecutionAgent
    {
        public MspecExecutionAgent(ITestRunnerExecutionContext context, IMessageBroker messageBroker)
        {
            Context = context;
            MessageBroker = messageBroker;

            Process = new MspecPreparedProcess();
        }

        public Task<int> Shutdown()
        {
            return Task.FromResult(0);
        }

        public string Id { get; }

        public Lifetime Lifetime { get; }

        public IPreparedProcess Process { get; }

        public IMessageBroker MessageBroker { get; }

        public object ActivationOptions { get; }

        public ITestRunnerExecutionContext Context { get; }

        public async Task RunTests(CancellationToken cancelCt, CancellationToken abortCt)
        {
            abortCt.Register(() => MessageBroker.Abort());

            var taskDepot = Context.Container.GetComponent<ITestRunnerRemoteTaskDepot>();

            var loader = Context.Adapter.GetTestAdapterLoader(Context);
            var container = Context.Adapter.GetTestContainer(Context);

            var tasks = taskDepot.GetRemoteTasks(Context.Run);
            
            await MessageBroker.Initialize(new RemoteAgentInitializationRequest(loader));
            await MessageBroker.RunTests(new TestRunRequest(container, tasks));
        }
    }
}
