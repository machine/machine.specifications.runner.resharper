﻿using System.Linq;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Persistence;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Elements
{
    public class MspecBehaviorTestElement : ClrUnitTestElement.FromMethod<MspecContextTestElement>
    {
        [UsedImplicitly]
        public MspecBehaviorTestElement()
        {
        }

        public MspecBehaviorTestElement(MspecContextTestElement parent, string fieldName, string? declaredInTypeShortName, string? explicitReason)
            : base($"{parent.TypeName.FullName}::{fieldName}", parent, fieldName, declaredInTypeShortName)
        {
            FieldName = fieldName;
            DisplayName = $"behaves like {fieldName.ToFormat()}";
            ExplicitReason = explicitReason;
        }

        public MspecContextTestElement Context => Parent;

        public override string Kind => "Behavior";

        [Persist]
        [UsedImplicitly]
        public string FieldName { get; set; }

        [Persist]
        [UsedImplicitly]
        public string DisplayName { get; set; }

        public override string ShortName => DisplayName;

        [Persist]
        [UsedImplicitly]
        public string? ExplicitReason { get; set; }

        protected override IDeclaredElement? GetTypeMember(ITypeElement declaredType)
        {
            return declaredType
                .EnumerateMembers<IField>(FieldName, declaredType.CaseSensitiveName)
                .FirstOrDefault();
        }
    }
}
