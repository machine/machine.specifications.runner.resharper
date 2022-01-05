using System;
using Machine.Specifications.Runner.ReSharper.Adapters.Models;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public class MspecReSharperId : IEquatable<MspecReSharperId>
    {
        public MspecReSharperId(IContext context)
        {
            Id = context.TypeName;
        }

        public MspecReSharperId(IContextSpecification specification)
        {
            Id = specification.IsBehavior
                ? $"{Self(specification.Context)}.{specification.ParentFieldName}.{specification.FieldName}"
                : $"{Self(specification.Context)}.{specification.FieldName}";
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

        public static string Self(IContext context)
        {
            return new MspecReSharperId(context).Id;
        }

        public static string Self(IContextSpecification specification)
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
                IContext context => new MspecReSharperId(context),
                IContextSpecification specification => new MspecReSharperId(specification),
                _ => throw new ArgumentOutOfRangeException(nameof(element))
            };
        }

        public static string? Parent(IMspecElement element)
        {
            return element switch
            {
                IContext => null,
                IContextSpecification {IsBehavior: false} specification => Self(specification.Context),
                IContextSpecification {IsBehavior: true} specification => $"{specification.ParentFieldName}",
                _ => throw new ArgumentOutOfRangeException(nameof(element))
            };
        }
    }
}
