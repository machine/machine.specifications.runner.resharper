using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.TaskRunnerFramework;
using Machine.Specifications.ReSharperRunner.Tasks;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.ReSharperRunner.Notifications
{
    public class BehaviorSpecificationRemoteTaskNotification : RemoteTaskNotification
    {
        private readonly TaskExecutionNode _node;
        private readonly BehaviorSpecificationTask _task;

        public BehaviorSpecificationRemoteTaskNotification(TaskExecutionNode node)
        {
            _node = node;
            _task = (BehaviorSpecificationTask)node.RemoteTask;
        }

        private string ContextTypeName => _task.ContextTypeName;

        private string ContainingType => _task.BehaviorTypeName;

        private string FieldName => _task.SpecificationFieldName;

        public override IEnumerable<RemoteTask> RemoteTasks
        {
            get { yield return _node.RemoteTask; }
        }

        public override bool Matches(object infoFromRunner, object maybeContext)
        {
            var context = maybeContext as ContextInfo;

            if (context == null)
                return false;

            var specification = infoFromRunner as SpecificationInfo;

            if (specification == null)
                return false;

            return ContextTypeName == context.TypeName &&
                   ContainingType == new NormalizedTypeName(specification.ContainingType) &&
                   FieldName == specification.FieldName;
        }

        public override string ToString()
        {
            return $"Behavior specification {ContainingType}.{FieldName} with {RemoteTasks.Count()} remote tasks";
        }
    }
}