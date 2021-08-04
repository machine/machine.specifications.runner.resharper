using System;
using System.Collections.Generic;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;

namespace Machine.Specifications.Runner.ReSharper.Elements
{
    public class MspecBehaviorSpecificationTestElement : MspecFieldTestElement, IEquatable<MspecBehaviorSpecificationTestElement>
    {
        public MspecBehaviorSpecificationTestElement(MspecServiceProvider services, UnitTestElementId id, IClrTypeName typeName, string fieldName, string? explicitReason)
            : base(services, id, typeName, fieldName, explicitReason)
        {
        }

        public MspecBehaviorTestElement? Behavior => Parent as MspecBehaviorTestElement;

        public override string Kind => "Behavior Specification";

        public override UnitTestElementDisposition GetDisposition()
        {
            var valid = Behavior?.GetDeclaredElement()?.IsValid();

            return valid.GetValueOrDefault()
                ? base.GetDisposition()
                : UnitTestElementDisposition.InvalidDisposition;
        }

        public override IEnumerable<UnitTestElementLocation> GetLocations(IDeclaredElement element)
        {
            return Behavior!.GetLocations(element);
        }

        public bool Equals(MspecBehaviorSpecificationTestElement? other)
        {
            return other != null &&
                   Equals(Id, other.Id) &&
                   Equals(TypeName, other.TypeName) &&
                   Equals(FieldName, other.FieldName);
        }
    }
}
