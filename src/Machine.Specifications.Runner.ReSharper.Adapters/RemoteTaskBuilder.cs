using System;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.ReSharper.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public static class RemoteTaskBuilder
    {
        public static MspecRemoteTask GetRemoteTask(RemoteTask task)
        {
            return task switch
            {
                MspecContextRemoteTask context => FromContext(context),
                MspecContextSpecificationRemoteTask specification => FromContextSpecification(specification),
                MspecBehaviorRemoteTask behavior => FromBehavior(behavior),
                MspecBehaviorSpecificationRemoteTask specification => FromBehaviorSpecification(specification),
                _ => throw new ArgumentOutOfRangeException(nameof(task))
            };
        }

        private static MspecRemoteTask FromContext(MspecContextRemoteTask task)
        {
            return MspecContextRemoteTask.ToServer(task.ContextTypeName, task.Subject, task.Tags, task.IgnoreReason);
        }

        private static MspecRemoteTask FromContextSpecification(MspecContextSpecificationRemoteTask task)
        {
            return MspecContextSpecificationRemoteTask.ToServer(
                task.ContextTypeName!,
                task.SpecificationFieldName!,
                task.Subject,
                task.Tags,
                task.IgnoreReason);
        }

        private static MspecRemoteTask FromBehavior(MspecBehaviorRemoteTask task)
        {
            return MspecBehaviorRemoteTask.ToServer(task.ContextTypeName, task.BehaviorFieldName, task.IgnoreReason);
        }

        private static MspecRemoteTask FromBehaviorSpecification(MspecBehaviorSpecificationRemoteTask task)
        {
            return MspecBehaviorSpecificationRemoteTask.ToServer(
                task.ContextTypeName!,
                task.BehaviorFieldName!,
                task.SpecificationFieldName!,
                task.IgnoreReason);
        }
    }
}
