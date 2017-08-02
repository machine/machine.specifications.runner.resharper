using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.TaskRunnerFramework;
using Machine.Specifications.ReSharperRunner.Tasks;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.ReSharperRunner.Notifications
{
    public class ContextSpecificationRemoteTaskNotification : RemoteTaskNotification
    {
        private readonly TaskExecutionNode _node;
        private readonly ContextSpecificationTask _task;

        public ContextSpecificationRemoteTaskNotification(TaskExecutionNode node)
        {
            _node = node;
            _task = (ContextSpecificationTask)node.RemoteTask;
        }

        private string ContainingType => _task.ContextTypeName;

        private string FieldName => _task.SpecificationFieldName;

        public override IEnumerable<RemoteTask> RemoteTasks
        {
            get { yield return _node.RemoteTask; }
        }

        public override bool Matches(object infoFromRunner, object maybeContext)
        {
            var specification = infoFromRunner as SpecificationInfo;

            if (specification == null)
                return false;

            return ContainingType == specification.ContainingType &&
                   FieldName == specification.FieldName;
        }

        public override string ToString()
        {
            return $"Context specification {ContainingType}.{FieldName} with {RemoteTasks.Count()} remote tasks";
        }
    }
}