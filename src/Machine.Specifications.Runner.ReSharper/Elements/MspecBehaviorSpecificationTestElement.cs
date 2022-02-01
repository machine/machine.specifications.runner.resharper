using JetBrains.Annotations;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Persistence;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Elements
{
    public class MspecBehaviorSpecificationTestElement : ClrUnitTestElement.Row, ITestCase, IMspecTestElement
    {
        [UsedImplicitly]
        public MspecBehaviorSpecificationTestElement()
        {
        }

        public MspecBehaviorSpecificationTestElement(MspecSpecificationTestElement parent, string fieldName, string? ignoreReason)
            : base($"{parent.NaturalId.TestId}.{fieldName}", parent)
        {
            FieldName = fieldName;
            DisplayName = fieldName.ToFormat();
            IgnoreReason = ignoreReason;
        }

        public override string Kind => "Behavior Specification";

        public bool IsNotRunnableStandalone => Origin == UnitTestElementOrigin.Dynamic;

        public MspecSpecificationTestElement Specification => (MspecSpecificationTestElement) Parent!;

        [Persist]
        [UsedImplicitly]
        public string FieldName { get; set; } = null!;

        [Persist]
        [UsedImplicitly]
        public string DisplayName { get; set; } = null!;

        public override string ShortName => DisplayName;

        [Persist]
        [UsedImplicitly]
        public string? IgnoreReason { get; set; }
    }
}
