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
    public class MessageBroker : IMessageBroker
    {
        private readonly TestRunnerHandler testRunnerHandler;

        private readonly Dictionary<Type, MessageHandler> messageHandlers = new Dictionary<Type, MessageHandler>();

        public MessageBroker(IAssemblyResolver resolver, IMessageHandlerMarker[] handlers)
        {
            testRunnerHandler = new TestRunnerHandler(this, resolver);

            InitializeMessageHandlers(handlers);
        }

        public async Task SendMessage(IMessage message)
        {
            switch (message)
            {
                case RemoteAgentInitializationRequest initialization:
                    testRunnerHandler.Execute(initialization);
                    break;

                case TestRunRequest testRun:
                    await testRunnerHandler.Execute(testRun).ConfigureAwait(false);
                    break;

                default:
                    await HandleMessage(message).ConfigureAwait(false);
                    break;
            }
        }

        public Task<TResult> SendMessage<TResult>(IMessage<TResult> message)
            where TResult : IAutoRegisterInProtocol
        {
            throw new NotSupportedException();
        }

        private Task HandleMessage(IMessage message)
        {
            if (!messageHandlers.TryGetValue(message.GetType(), out var handler))
            {
                return Task.CompletedTask;
            }

            if (handler.Method.Invoke(handler.Handler, new object[] {message}) is Task task)
            {
                return task;
            }

            return Task.CompletedTask;
        }

        private void InitializeMessageHandlers(IMessageHandlerMarker[] handlers)
        {
            foreach (var handler in handlers)
            {
                var asyncHandlers = handler.GetType()
                    .GetInterfaces()
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
