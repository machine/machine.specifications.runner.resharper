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

        public MspecBehaviorSpecificationTestElement(MspecContextSpecificationTestElement parent, string fieldName, string behaviorType, string? ignoreReason)
            : base($"{parent.Context.TypeName.FullName}.{fieldName}", parent)
        {
            FieldName = fieldName;
            BehaviorType = behaviorType;
            DisplayName = fieldName.ToFormat();
            IgnoreReason = ignoreReason;
        }

        public override string Kind => "Behavior Specification";

        public bool IsNotRunnableStandalone => Origin == UnitTestElementOrigin.Dynamic;

        public MspecContextSpecificationTestElement Specification => (MspecContextSpecificationTestElement) Parent;

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
    }
}
