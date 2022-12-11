using System;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;

namespace Machine.Specifications.Runner.ReSharper.Adapters;

public class MspecReSharperId : IEquatable<MspecReSharperId>
{
    public MspecReSharperId(IContextElement context)
    {
        Id = context.TypeName;
    }

    public MspecReSharperId(IBehaviorElement behavior)
    {
        Id = $"{behavior.Context.TypeName}.{behavior.FieldName}";
    }

    public MspecReSharperId(ISpecificationElement specification)
    {
        Id = specification.Behavior != null
            ? $"{specification.Context.TypeName}.{specification.Behavior.FieldName}.{specification.FieldName}"
            : $"{specification.Context.TypeName}.{specification.FieldName}";
    }

    public string Id { get; }

    public bool Equals(MspecReSharperId? other)
    {
        return other != null && Id == other.Id;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as MspecReSharperId);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public override string ToString()
    {
        return $"ReSharperId({Id})";
    }

    public static string Self(IContextElement context)
    {
        return new MspecReSharperId(context).Id;
    }

    public static string Self(IBehaviorElement behavior)
    {
        return new MspecReSharperId(behavior).Id;
    }

    public static string Self(ISpecificationElement specification)
    {
        return new MspecReSharperId(specification).Id;
    }

    public static string Self(IMspecElement element)
    {
        return Create(element).Id;
    }

    public static MspecReSharperId Create(IMspecElement element)
    {
        return element switch
        {
            IContextElement context => new MspecReSharperId(context),
            ISpecificationElement specification => new MspecReSharperId(specification),
            IBehaviorElement behavior => new MspecReSharperId(behavior),
            _ => throw new ArgumentOutOfRangeException(nameof(element))
        };
    }

    public static string? Parent(IMspecElement element)
    {
        return element switch
        {
            IContextElement => null,
            IBehaviorElement behavior => Self(behavior.Context),
            ISpecificationElement {Behavior: not null} specification => Self(specification.Behavior),
            ISpecificationElement {Behavior: null} specification => Self(specification.Context),
            _ => throw new ArgumentOutOfRangeException(nameof(element))
        };
    }
}
