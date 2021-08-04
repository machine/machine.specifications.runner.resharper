using System;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.UnitTestFramework;

namespace Machine.Specifications.Runner.ReSharper.Elements
{
    public class MspecBehaviorTestElement : MspecFieldTestElement, IEquatable<MspecBehaviorTestElement>
    {
        public MspecBehaviorTestElement(MspecServiceProvider services, UnitTestElementId id, IClrTypeName typeName, string fieldName, string? explicitReason)
            : base(services, id, typeName, fieldName, explicitReason)
        {
        }

        public MspecContextTestElement? Context => Parent as MspecContextTestElement;

        public override string Kind => "Behavior";

        protected override string GetTitlePrefix()
        {
            return "behaves like";
        }

        public bool Equals(MspecBehaviorTestElement? other)
        {
            return other != null &&
                   Equals(Id, other.Id) &&
                   Equals(TypeName, other.TypeName) &&
                   Equals(FieldName, other.FieldName);
        }
    }
}
