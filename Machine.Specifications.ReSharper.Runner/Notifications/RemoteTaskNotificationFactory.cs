using System.Collections.Generic;
using JetBrains.ReSharper.TaskRunnerFramework;

using Machine.Specifications.ReSharperRunner.Tasks;

namespace Machine.Specifications.ReSharperRunner.Runners.Notifications
{
    public class RemoteTaskNotificationFactory
    {
        public RemoteTaskNotification CreateTaskNotification(TaskExecutionNode node, ICollection<string> contexts)
        {
            var remoteTask = node.RemoteTask;

            if (remoteTask is RunAssemblyTask)
            {
                return new AssemblyRemoteTaskNotification(node);
            }

            if (remoteTask is ContextTask)
            {
                return new ContextRemoteTaskNotification(node);
            }

            if (remoteTask is ContextSpecificationTask)
            {
                return new ContextSpecificationRemoteTaskNotification(node);
            }

            if (remoteTask is BehaviorSpecificationTask)
            {
                return new BehaviorSpecificationRemoteTaskNotification(node);
            }

            return new SilentRemoteTaskNotification();
        }
    }
}