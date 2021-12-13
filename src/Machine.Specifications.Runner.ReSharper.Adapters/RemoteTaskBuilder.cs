using System;
using Machine.Specifications.Runner.ReSharper.Adapters.Models;
using Machine.Specifications.Runner.ReSharper.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public static class RemoteTaskBuilder
    {
        public static MspecRemoteTask GetRemoteTask(IMspecElement element)
        {
            return element switch
            {
                IContext context => FromContext(context),
                IContextSpecification {IsBehavior: false} specification => FromSpecification(specification),
                IContextSpecification {IsBehavior: true} specification => FromBehavior(specification),
                _ => throw new ArgumentOutOfRangeException(nameof(element))
            };
        }

        private static MspecRemoteTask FromContext(IContext context)
        {
            return MspecContextRemoteTask.ToServer(MspecReSharperId.Self(context), context.Subject, null, null);
        }

        private static MspecRemoteTask FromSpecification(IContextSpecification specification)
        {
            return MspecContextSpecificationRemoteTask.ToServer(specification.ContainingType, specification.FieldName, null, null, null);
        }

        private static MspecRemoteTask FromBehavior(IContextSpecification specification)
        {
            return MspecBehaviorSpecificationRemoteTask.ToServer(
                specification.Context.TypeName,
                specification.Context.TypeName,
                specification.ContainingType,
                specification.FieldName,
                null);
        }
    }
}
