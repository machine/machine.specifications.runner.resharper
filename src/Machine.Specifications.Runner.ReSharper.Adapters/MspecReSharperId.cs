using System;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.ReSharper.Tasks;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public class MspecReSharperId : IEquatable<MspecReSharperId>
    {
        private MspecReSharperId(string id)
        {
            Id = id;
        }

        public MspecReSharperId(ContextInfo context)
        {
            Id = context.TypeName;
        }

        public MspecReSharperId(SpecificationInfo specification)
        {
            Id = $"{specification.ContainingType}.{specification.FieldName}";
        }

        public MspecReSharperId(ContextInfo context, SpecificationInfo behaviorSpecification)
        {
            Id = $"{context.TypeName}.{behaviorSpecification.FieldName}";
        }

        public MspecReSharperId(MspecContextRemoteTask context)
        {
            Id = context.ContextTypeName;
        }

        public MspecReSharperId(MspecContextSpecificationRemoteTask specification)
        {
            Id = $"{specification.ContextTypeName}.{specification.SpecificationFieldName}";
        }

        public MspecReSharperId(MspecBehaviorSpecificationRemoteTask specification)
        {
            Id = $"{specification.ContextTypeName}.{specification.SpecificationFieldName}";
        }

        public MspecReSharperId(Context context)
        {
            Id = context.TypeName;
        }

        public MspecReSharperId(Specification specification)
        {
            Id = specification.IsBehavior()
                ? $"{specification.Context.TypeName}.{specification.ContainingType}.{specification.FieldName}"
                : $"{specification.Context.TypeName}.{specification.FieldName}";
        }

        public string Id { get; }

        public static string Self(ContextInfo context)
        {
            return new MspecReSharperId(context).Id;
        }

        public static string Self(SpecificationInfo specification)
        {
            return new MspecReSharperId(specification).Id;
        }

        public static string Self(ContextInfo context, SpecificationInfo behaviorSpecification)
        {
            return new MspecReSharperId(context, behaviorSpecification).Id;
        }

        public static string Self(Specification specification)
        {
            return new MspecReSharperId(specification).Id;
        }

        public static string Self(Context context)
        {
            return new MspecReSharperId(context).Id;
        }

        public static string Self(TestElement element)
        {
            return element switch
            {
                Context context => Self(context),
                Specification specification => Self(specification),
                _ => throw new ArgumentOutOfRangeException(nameof(element))
            };
        }

        public static MspecReSharperId Create(RemoteTask task)
        {
            return task switch
            {
                MspecContextRemoteTask context => new MspecReSharperId(context),
                MspecContextSpecificationRemoteTask specification => new MspecReSharperId(specification),
                MspecBehaviorSpecificationRemoteTask specification => new MspecReSharperId(specification),
                _ => throw new ArgumentOutOfRangeException(nameof(task))
            };
        }

        public static string? Parent(TestElement element)
        {
            return element switch
            {
                Context context => null,
                Specification specification when specification.IsBehavior() => new MspecReSharperId($"{specification.Context.TypeName}.{specification.ContainingType}").Id,
                Specification specification when !specification.IsBehavior() => Self(specification.Context),
                _ => throw new ArgumentOutOfRangeException(nameof(element))
            };
        }

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
    }
}
