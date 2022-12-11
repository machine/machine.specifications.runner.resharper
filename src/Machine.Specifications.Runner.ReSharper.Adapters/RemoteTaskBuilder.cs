using System;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.ReSharper.Tasks;

namespace Machine.Specifications.Runner.ReSharper.Adapters;

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

    private static MspecRemoteTask FromContext(IContextElement context)
    {
        return MspecContextRemoteTask.ToServer(MspecReSharperId.Self(context), context.Subject, null, context.IgnoreReason);
    }

    private static MspecRemoteTask FromBehavior(IBehaviorElement behavior)
    {
        return MspecSpecificationRemoteTask.ToServer(behavior.Context.TypeName, behavior.FieldName, behavior.TypeName, null, null, behavior.IgnoreReason);
    }

    private static MspecRemoteTask FromSpecification(ISpecificationElement specification)
    {
        if (specification.Behavior != null)
        {
            return MspecBehaviorSpecificationRemoteTask.ToServer(
                $"{specification.Context.TypeName}.{specification.Behavior.FieldName}",
                specification.Context.TypeName,
                specification.FieldName,
                specification.IgnoreReason);
        }

        return MspecSpecificationRemoteTask.ToServer(specification.Context.TypeName, specification.FieldName, null, null, null, specification.IgnoreReason);
    }
}
