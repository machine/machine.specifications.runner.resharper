using System.Linq;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Persistence;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Elements;

public class MspecSpecificationTestElement : ClrUnitTestElement.FromMethod<MspecContextTestElement>, IMspecTestElement
{
    [UsedImplicitly]
    public MspecSpecificationTestElement()
    {
    }

    public MspecSpecificationTestElement(MspecContextTestElement parent, string fieldName, string? behaviorType, string? declaredInTypeShortName, string? ignoreReason)
        : base($"{parent.TypeName.FullName}.{fieldName}", parent, fieldName, declaredInTypeShortName)
    {
        FieldName = fieldName;
        BehaviorType = behaviorType;
        DisplayName = fieldName.ToFormat();
        IgnoreReason = ignoreReason;
    }

    public MspecContextTestElement Context => Parent;

    public override string Kind => "Specification";

    [Persist]
    [UsedImplicitly]
    public string FieldName { get; set; } = null!;

    [Persist]
    [UsedImplicitly]
    public string? BehaviorType { get; set; }

    [Persist]
    [UsedImplicitly]
    public string DisplayName { get; set; } = null!;

    public override string ShortName => DisplayName;

    [Persist]
    [UsedImplicitly]
    public string? IgnoreReason { get; set; }

    protected override IDeclaredElement? GetTypeMember(ITypeElement declaredType)
    {
        return declaredType
            .EnumerateMembers<IField>(FieldName, declaredType.CaseSensitiveName)
            .FirstOrDefault();
    }
}
