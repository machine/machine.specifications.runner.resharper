using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.TaskRunnerFramework;
using Machine.Specifications.ReSharperRunner.Tasks;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.ReSharperRunner.Notifications
{
    public class ContextRemoteTaskNotification : RemoteTaskNotification
    {
        private readonly TaskExecutionNode _node;
        private readonly ContextTask _task;

        public ContextRemoteTaskNotification(TaskExecutionNode node)
        {
            _node = node;
            _task = (ContextTask)node.RemoteTask;
        }

        private string ContainingType => _task.ContextTypeName;

        public override IEnumerable<RemoteTask> RemoteTasks
        {
            get { yield return _node.RemoteTask; }
        }

        public override bool Matches(object infoFromRunner, object maybeContext)
        {
            var context = infoFromRunner as ContextInfo;

            if (context == null)
                return false;

            return ContainingType == context.TypeName;
        }

        public override string ToString()
        {
            return $"Context {ContainingType} with {RemoteTasks.Count()} remote tasks";
        }
    }
}