using System;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Elements
{
    public class MspecContextTestElement : MspecTestElement, IEquatable<MspecContextTestElement>
    {
        public MspecContextTestElement(MspecServiceProvider services, UnitTestElementId id, IClrTypeName typeName, string? subject, string? explicitReason)
            : base(services, id, typeName, explicitReason)
        {
            Subject = subject;
            ShortName = typeName.ShortName.ToFormat();
        }

        public override string Kind => "Context";

        public string? Subject { get; }

        protected override string GetPresentation()
        {
            var display = TypeName.ShortName.ToFormat();

            return string.IsNullOrEmpty(Subject)
                ? display
                : $"{Subject}, {display}";
        }

        public override IDeclaredElement? GetDeclaredElement()
        {
            return GetDeclaredType();
        }

        public bool Equals(MspecContextTestElement? other)
        {
            return other != null &&
                   Equals(Id, other.Id) &&
                   Equals(TypeName, other.TypeName);
        }
    }
}
