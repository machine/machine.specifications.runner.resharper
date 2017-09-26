using System.Linq;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.UnitTestFramework;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.ReSharperProvider.Presentation
{
    public abstract class FieldElement : Element
    {
        protected FieldElement(
            IUnitTestElement parent,
            IClrTypeName declaringTypeName,
            MspecServiceProvider serviceProvider,
            string fieldName,
            bool isIgnored)
            : base(parent, declaringTypeName, serviceProvider, isIgnored || parent.Explicit)
        {
            FieldName = fieldName;
        }

        public override string ShortName => FieldName;

        public string FieldName { get; }

        protected override string GetPresentation()
        {
            var prefix = GetTitlePrefix();
            var title = string.IsNullOrEmpty(GetTitlePrefix()) ? string.Empty : " ";

            return $"{prefix}{title}{FieldName.ToFormat()}";
        }

        public override IDeclaredElement GetDeclaredElement()
        {
            ITypeElement declaredType = GetDeclaredType();

            return declaredType?.EnumerateMembers(FieldName, true)
                .OfType<IField>()
                .FirstOrDefault();
        }
    }
}