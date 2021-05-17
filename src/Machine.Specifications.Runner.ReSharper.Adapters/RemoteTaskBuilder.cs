using System;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.ReSharper.Tasks;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public static class RemoteTaskBuilder
    {
        private static readonly string[] Empty = new string[0];

        public static MspecRemoteTask GetRemoteTask(object element)
        {
            return element switch
            {
                ContextInfo context => FromContext(context),
                SpecificationInfo specification => FromSpecification(specification),
                _ => throw new ArgumentOutOfRangeException(nameof(element))
            };
        }

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
            return MspecContextRemoteTask.ToServer(task.ContextTypeName, task.Subject, task.Tags);
        }

        private static MspecRemoteTask FromContextSpecification(MspecContextSpecificationRemoteTask task)
        {
            return MspecContextSpecificationRemoteTask.ToServer(
                task.ContextTypeName,
                task.SpecificationFieldName,
                task.DisplayName,
                task.Subject,
                task.Tags);
        }

        private static MspecRemoteTask FromBehavior(MspecBehaviorRemoteTask task)
        {
            return MspecBehaviorRemoteTask.ToServer(task.ContextTypeName, task.BehaviorFieldName);
        }

        private static MspecRemoteTask FromBehaviorSpecification(MspecBehaviorSpecificationRemoteTask task)
        {
            return MspecBehaviorSpecificationRemoteTask.ToServer(
                task.ContextTypeName,
                task.BehaviorFieldName,
                task.SpecificationFieldName);
        }

        private static MspecRemoteTask FromContext(ContextInfo context)
        {
            return MspecContextRemoteTask.ToServer(context.TypeName, context.Name, Empty);
        }

        private static MspecRemoteTask FromSpecification(SpecificationInfo specification)
        {
            return MspecContextSpecificationRemoteTask.ToServer(
                specification.ContainingType,
                specification.FieldName,
                specification.Name,
                specification.Name,
                Empty);
        }
    }
}
