using System;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.ReSharper.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public static class RemoteTaskBuilder
    {
        public static MspecRemoteTask GetRemoteTask(IMspecElement element)
        {
            return element switch
            {
                IContextElement context => FromContext(context),
                ISpecificationElement {IsBehavior: false} specification => FromSpecification(specification),
                ISpecificationElement {IsBehavior: true} specification => FromBehavior(specification),
                _ => throw new ArgumentOutOfRangeException(nameof(element))
            };
        }

        private static MspecRemoteTask FromContext(IContextElement context)
        {
            return MspecContextRemoteTask.ToServer(MspecReSharperId.Self(context), context.Subject, null, null);
        }

        private static MspecRemoteTask FromSpecification(ISpecificationElement specification)
        {
            var behaviorType = specification.IsBehavior
                ? specification.Context.TypeName
                : null;

            return MspecContextSpecificationRemoteTask.ToServer(specification.ContainingType, specification.FieldName, behaviorType, null, null, null);
        }

        private static MspecRemoteTask FromBehavior(ISpecificationElement specification)
        {
            var parentId = $"{specification.Context.TypeName}.{specification.BehaviorSpecification!.FieldName}";

            return MspecBehaviorSpecificationRemoteTask.ToServer(
                parentId,
                specification.Context.TypeName,
                specification.ContainingType,
                specification.FieldName,
                null);
        }
    }
}
