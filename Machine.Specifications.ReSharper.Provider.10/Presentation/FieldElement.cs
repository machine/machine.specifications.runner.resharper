using JetBrains.ReSharper.UnitTestFramework;

namespace Machine.Specifications.ReSharperProvider.Presentation
{
    using JetBrains.Metadata.Reader.API;
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.Psi.Util;
    using Runner.Utility;
    using System;
    using System.Linq;

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
            var presentation = String.Format("{0}{1}{2}",
                                             this.GetTitlePrefix(),
                                             String.IsNullOrEmpty(this.GetTitlePrefix()) ? String.Empty : " ",
                                             this.FieldName.ToFormat());

#if DEBUG
            presentation += String.Format(" ({0})", this.Id);
#endif

            return presentation;
        }

        public override IDeclaredElement GetDeclaredElement()
        {
            ITypeElement declaredType = this.GetDeclaredType();
            if (declaredType == null)
            {
                return null;
            }

            return declaredType
              .EnumerateMembers(this.FieldName, false)
              .FirstOrDefault(member => member as IField != null);
        }
    }
}