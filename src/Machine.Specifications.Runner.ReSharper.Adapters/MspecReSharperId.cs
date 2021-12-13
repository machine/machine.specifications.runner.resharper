﻿using System;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using Machine.Specifications.Runner.ReSharper.Adapters.Elements;
using Machine.Specifications.Runner.ReSharper.Tasks;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Adapters
{
    public class MspecReSharperId : IEquatable<MspecReSharperId>
    {
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
