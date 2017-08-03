using System.Linq;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.ReSharperProvider.Presentation
{
    public abstract class FieldElement : Element
    {
        protected FieldElement(MSpecUnitTestProvider provider,
                               Element parent,
                               IClrTypeName declaringTypeName,
                               UnitTestingCachingService cachingService,
                               IUnitTestElementManager elementManager,
                               string fieldName,
                               bool isIgnored)
            : base(provider, parent, declaringTypeName, cachingService, elementManager, isIgnored || parent.Explicit)
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