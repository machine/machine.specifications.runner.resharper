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
            UnitTestElementId id,
            IUnitTestElement parent,
            IClrTypeName typeName,
            MspecServiceProvider serviceProvider,
            string fieldName,
            bool isIgnored)
            : base(id, parent, typeName, serviceProvider, isIgnored || parent.Explicit)
        {
            FieldName = fieldName;
        }

        public override string ShortName => FieldName;

        public string FieldName { get; }

        protected virtual string GetTitlePrefix()
        {
            return string.Empty;
        }

        protected override string GetPresentation()
        {
            var value = $"{GetTitlePrefix()} {FieldName.ToFormat()}";

            return value.Trim();
        }

        public override IDeclaredElement GetDeclaredElement()
        {
            return GetDeclaredType()?
                .EnumerateMembers(FieldName, true)
                .OfType<IField>()
                .FirstOrDefault();
        }
    }
}