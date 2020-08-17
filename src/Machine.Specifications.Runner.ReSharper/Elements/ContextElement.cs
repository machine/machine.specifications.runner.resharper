using System;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Elements
{
    public class ContextElement : Element, IEquatable<ContextElement>
    {
        private readonly string subject;

        public ContextElement(
            UnitTestElementId id,
            IClrTypeName typeName,
            MspecServiceProvider serviceProvider,
            string subject,
            bool isIgnored)
            : base(id, null, typeName, serviceProvider, isIgnored)
        {
            this.subject = subject;
        }

        public override string ShortName => Kind + GetPresentation();

        public override string Kind => "Context";

        protected override string GetPresentation()
        {
            var display = TypeName.ShortName.ToFormat();

            return string.IsNullOrEmpty(subject)
                ? display
                : $"{subject}, {display}";
        }

        public override IDeclaredElement GetDeclaredElement()
        {
            return GetDeclaredType();
        }

        public bool Equals(ContextElement other)
        {
            return other != null &&
                   Equals(Id, other.Id) &&
                   Equals(TypeName, other.TypeName);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ContextElement);
        }

        public override int GetHashCode()
        {
            return HashCode
                .Of(Id)
                .And(TypeName);
        }
    }
}
