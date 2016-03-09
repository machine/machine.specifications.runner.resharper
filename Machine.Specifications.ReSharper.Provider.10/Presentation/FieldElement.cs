using JetBrains.ReSharper.UnitTestFramework;

namespace Machine.Specifications.ReSharperProvider.Presentation
{
    using System;
    using System.Linq;
    using JetBrains.Metadata.Reader.API;
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.Psi.Util;
    using Runner.Utility;

    public abstract class FieldElement : Element
    {
        readonly string _fieldName;

        protected FieldElement(MSpecUnitTestProvider provider,
                               Element parent,
                               IClrTypeName declaringTypeName,
                               UnitTestingCachingService cachingService,
                               string fieldName,
                               bool isIgnored)
            : base(provider, parent, declaringTypeName, cachingService, isIgnored || parent.Explicit)
        {
            this._fieldName = fieldName;
        }

        public override string ShortName
        {
            get { return this.FieldName; }
        }

        public string FieldName
        {
            get { return this._fieldName; }
        }

        public override string GetPresentation()
        {
            return String.Format("{0}{1}{2}",
                this.GetTitlePrefix(),
                String.IsNullOrEmpty(this.GetTitlePrefix()) ? String.Empty : " ",
                this.FieldName.ToFormat());
        }

        public override IDeclaredElement GetDeclaredElement()
        {
            ITypeElement declaredType = this.GetDeclaredType();
            if (declaredType == null)
            {
                return null;
            }

            return declaredType
                .EnumerateMembers(this.FieldName, true)
                .OfType<IField>()
                .Single();
        }
    }
}