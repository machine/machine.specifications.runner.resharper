using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.UnitTestFramework.TestRunner;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    [SolutionComponent]
    public class MspecTestRunnerAgentManager : ITestRunnerAgentManager
    {
        private readonly IMessageBroker messageBroker;

        public MspecTestRunnerAgentManager(IMessageBroker messageBroker)
        {
            this.messageBroker = messageBroker;
        }

        public Task<ITestRunnerExecutionAgent> GetExecutionAgent(ITestRunnerExecutionContext context, CancellationToken ct)
        {
            return Task.FromResult<ITestRunnerExecutionAgent>(new MspecExecutionAgent(context, messageBroker));
        }

        public Task<ITestRunnerDiscoveryAgent> GetDiscoveryAgent(ITestRunnerDiscoveryContext context, CancellationToken ct)
        {
            throw new NotSupportedException();
        }
    }
}
