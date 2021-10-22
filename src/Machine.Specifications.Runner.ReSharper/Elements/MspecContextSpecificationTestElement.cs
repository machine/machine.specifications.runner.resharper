using System.Linq;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Elements
{
    public class MspecContextSpecificationTestElement : ClrUnitTestElement.FromMethod<MspecContextTestElement>
    {
        [UsedImplicitly]
        public MspecContextSpecificationTestElement()
        {
        }

        public MspecContextSpecificationTestElement(MspecContextTestElement parent, string fieldName, string? declaredInTypeShortName, string? explicitReason)
            : base($"{parent.TypeName.FullName}::{fieldName}", parent, fieldName, declaredInTypeShortName)
        {
            FieldName = fieldName;
            ShortName = fieldName.ToFormat();
            ExplicitReason = explicitReason;
        }

        public MspecContextTestElement Context => Parent;

        public override string Kind => "Specification";

        public string FieldName { get; }

        public override string ShortName { get; }

        public string? ExplicitReason { get; }

        protected override IDeclaredElement? GetTypeMember(ITypeElement declaredType)
        {
            return declaredType
                .EnumerateMembers<IField>(ShortName, declaredType.CaseSensitiveName)
                .FirstOrDefault();
        }
    }
}
