using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Elements
{
    public class MspecBehaviorSpecificationTestElement : ClrUnitTestElement.Row, ITestCase
    {
        [UsedImplicitly]
        public MspecBehaviorSpecificationTestElement()
        {
        }

        public MspecBehaviorSpecificationTestElement(MspecContextTestElement context, MspecBehaviorTestElement behavior, string fieldName, string? explicitReason)
            : base($"{context.TypeName.FullName}::{fieldName}", behavior)
        {
            FieldName = fieldName;
            ShortName = fieldName.ToFormat();
            ExplicitReason = explicitReason;
        }

        public MspecBehaviorTestElement? Behavior => Parent as MspecBehaviorTestElement;

        public override string Kind => "Behavior Specification";

        public bool IsNotRunnableStandalone => Origin == UnitTestElementOrigin.Dynamic;

        public string FieldName { get; }

        public string? ExplicitReason { get; }

        public override string ShortName { get; }

        public override IEnumerable<UnitTestElementLocation> GetLocations()
        {
            return Behavior!.GetLocations();
        }
    }
}
