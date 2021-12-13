using System;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
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
                MspecBehaviorSpecificationRemoteTask specification => FromBehaviorSpecification(specification),
                _ => throw new ArgumentOutOfRangeException(nameof(task))
            };
        }

        public static MspecRemoteTask GetRemoteTask(TestElement element)
        {
            return element switch
            {
                Context context => FromContext(context),
                Specification specification when !specification.IsBehavior() => FromSpecification(specification),
                Specification specification when specification.IsBehavior() => FromBehavior(specification),
                _ => throw new ArgumentOutOfRangeException(nameof(element))
            };
        }

        private static MspecRemoteTask FromContext(Context context)
        {
            return MspecContextRemoteTask.ToServer(context.TypeName, context.Subject, null, null);
        }

        private static MspecRemoteTask FromSpecification(Specification specification)
        {
            return MspecContextSpecificationRemoteTask.ToServer(specification.Context.TypeName, specification.FieldName, specification.Context.Subject, null, null);
        }

        private static MspecRemoteTask FromBehavior(Specification specification)
        {
            var parent = $"{specification.Context.TypeName}.{specification.ContainingType}";

            return MspecBehaviorSpecificationRemoteTask.ToServer(parent, specification.Context.TypeName, specification.FieldName, null);
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

        private static MspecRemoteTask FromBehaviorSpecification(MspecBehaviorSpecificationRemoteTask task)
        {
            return MspecBehaviorSpecificationRemoteTask.ToServer(
                task.ParentId,
                task.ContextTypeName!,
                task.SpecificationFieldName!,
                task.IgnoreReason);
        }
    }
}
