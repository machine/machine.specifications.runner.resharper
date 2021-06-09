using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.ReSharper.TestRunner.Abstractions;
using JetBrains.ReSharper.TestRunner.Abstractions.Isolation;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;

namespace Machine.Specifications.Runner.ReSharper.Tests
{
    public class MspecMessageBroker : IMessageBroker
    {
        private readonly MspecTestRemoteAgent agent;

        private readonly Dictionary<Type, MessageHandler> messageHandlers = new Dictionary<Type, MessageHandler>();

        public MspecMessageBroker(IAssemblyResolver resolver, IMessageHandlerMarker[] handlers)
        {
            agent = new MspecTestRemoteAgent(this, resolver);

            InitializeMessageHandlers(handlers);
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

                default:
                    await HandleMessage(message);
                    break;
            }
        }

        public Task<TResult> SendMessage<TResult>(IMessage<TResult> message)
            where TResult : IAutoRegisterInProtocol
        {
            throw new NotSupportedException();
        }

        private async Task HandleMessage(IMessage message)
        {
            if (messageHandlers.TryGetValue(message.GetType(), out var handler))
            {
                if (handler.Method.Invoke(handler.Handler, new object[] {message}) is Task task)
                {
                    await task;
                }
            }
        }

        private void InitializeMessageHandlers(IMessageHandlerMarker[] handlers)
        {
            foreach (var handler in handlers)
            {
                var asyncHandlers = handler.GetType().GetInterfaces()
                    .Where(x => x.IsGenericType)
                    .Where(x => x.GetGenericTypeDefinition() == typeof(IAsyncMessageHandler<>));

                foreach (var asyncHandler in asyncHandlers)
                {
                    var messageType = asyncHandler.GetGenericArguments().First();
                    var method = asyncHandler.GetMethod("Execute", new[] {messageType});

                    messageHandlers[messageType] = new MessageHandler(handler, method);
                }
            }
        }

        private class MessageHandler
        {
            public MessageHandler(IMessageHandlerMarker handler, MethodInfo method)
            {
                Handler = handler;
                Method = method;
            }

            public IMessageHandlerMarker Handler { get; }

            public MethodInfo Method { get; }
        }
    }
}
