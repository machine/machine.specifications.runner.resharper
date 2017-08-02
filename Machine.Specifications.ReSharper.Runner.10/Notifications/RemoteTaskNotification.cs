using System.Collections.Generic;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Machine.Specifications.ReSharperRunner.Notifications
{
    public abstract class RemoteTaskNotification
    {
        public abstract IEnumerable<RemoteTask> RemoteTasks { get; }

        public abstract bool Matches(object infoFromRunner, object maybeContext);
    }
}