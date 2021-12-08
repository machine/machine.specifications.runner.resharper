using System;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.ReSharper.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public static class RemoteTaskBuilder
    {
        public static MspecRemoteTask GetRemoteTask(RemoteTask task)
        {
            switch (task)
            {
                case MspecContextRemoteTask context:
                    return FromContext(context);

                case MspecContextSpecificationRemoteTask specification:
                    return FromContextSpecification(specification);

                case MspecBehaviorRemoteTask behavior:
                    return FromBehavior(behavior);

                case MspecBehaviorSpecificationRemoteTask specification:
                    return FromBehaviorSpecification(specification);

                default:
                    throw new ArgumentOutOfRangeException(nameof(task));
            }
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
