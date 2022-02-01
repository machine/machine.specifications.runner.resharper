using System;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.ReSharper.Tasks;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public static class RemoteTaskBuilder
    {
        public static MspecRemoteTask GetRemoteTask(IMspecElement element)
        {
            return element switch
            {
                IContextElement context => FromContext(context),
                IBehaviorElement behavior => FromBehavior(behavior),
                ISpecificationElement specification => FromSpecification(specification),
                _ => throw new ArgumentOutOfRangeException(nameof(element))
            };
        }

        public static MspecRemoteTask GetRemoteTask(ContextInfo context)
        {
            return MspecContextRemoteTask.ToServer(context.TypeName, context.Concern, null, null);
        }

        public static MspecRemoteTask GetRemoteTask(ContextInfo context, SpecificationInfo specification)
        {
            var isBehavior = context.TypeName != specification.ContainingType;

            var behaviorType = isBehavior
                ? context.TypeName
                : null;

            return MspecSpecificationRemoteTask.ToServer(specification.ContainingType, specification.FieldName, behaviorType, null, null, null);
        }

        private static MspecRemoteTask FromContext(IContextElement context)
        {
            return MspecContextRemoteTask.ToServer(MspecReSharperId.Self(context), context.Subject, null, null);
        }

        private static MspecRemoteTask FromBehavior(IBehaviorElement behavior)
        {
            return MspecBehaviorSpecificationRemoteTask.ToServer(
                $"{behavior.Context.TypeName}.{behavior.FieldName}",
                behavior.Context.TypeName,
                behavior.FieldName,
                null);
        }

        private static MspecRemoteTask FromSpecification(ISpecificationElement specification)
        {
            var behaviorType = specification.Behavior?.TypeName;

            return MspecSpecificationRemoteTask.ToServer(specification.Context.TypeName, specification.FieldName, behaviorType, null, null, null);
        }
    }
}
