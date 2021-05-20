using System;
using System.Threading.Tasks;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    [SolutionComponent]
    public class MspecMessageBroker : IMessageBroker
    {
        private readonly MspecTestRemoteAgent agent;

        public MspecMessageBroker(ITestAdapterLoadContextFactory contextFactory)
        {
            agent = new MspecTestRemoteAgent(this, contextFactory);
        }

        public async Task SendMessage(IMessage message)
        {
            switch (message)
            {
                case RemoteAgentInitializationRequest initialization:
                    agent.Execute(initialization);
                    break;

                case TestRunRequest testRun:
                    await agent.Execute(testRun);
                    break;
            }
        }

        public Task<TResult> SendMessage<TResult>(IMessage<TResult> message)
            where TResult : IAutoRegisterInProtocol
        {
            throw new NotSupportedException();
        }
    }
}
