using System.Linq;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.UnitTestFramework.Elements;

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
            ExplicitReason = explicitReason;
        }

        public MspecContextTestElement Context => Parent;

        public override string Kind => "Behavior";

        public string FieldName { get; }

        public string? ExplicitReason { get; }

        public override string GetPresentation()
        {
            return $"behaves like {ShortName}";
        }

        protected override IDeclaredElement? GetTypeMember(ITypeElement declaredType)
        {
            return declaredType
                .EnumerateMembers<IField>(ShortName, declaredType.CaseSensitiveName)
                .FirstOrDefault();
        }
    }
}
