using System;
using System.Linq;
using Machine.Specifications.Runner.ReSharper.Adapters.Models;
using Machine.Specifications.Runner.ReSharper.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public static class RemoteTaskBuilder
    {
        public static MspecRemoteTask GetRemoteTask(RemoteTaskDepot depot, IMspecElement element)
        {
            return element switch
            {
                IContext context => FromContext(context),
                IContextSpecification {IsBehavior: false} specification => FromSpecification(specification),
                IContextSpecification {IsBehavior: true} specification => FromBehavior(depot, specification),
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

        private static MspecRemoteTask FromBehavior(RemoteTaskDepot depot, IContextSpecification specification)
        {
            var parentSpecification = depot.GetTasks()
                .OfType<MspecContextSpecificationRemoteTask>()
                .FirstOrDefault(x => x.BehaviorType == specification.ContainingType);

            var parentId = $"{specification.Context.TypeName}.{parentSpecification.SpecificationFieldName}";

            return MspecBehaviorSpecificationRemoteTask.ToServer(
                parentId,
                specification.Context.TypeName,
                specification.ContainingType,
                specification.FieldName,
                null);
        }
    }
}
