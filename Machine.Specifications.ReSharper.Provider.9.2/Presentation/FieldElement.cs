﻿namespace Machine.Specifications.ReSharperProvider.Presentation
{
    using System;
    using System.Linq;
    using JetBrains.Metadata.Reader.API;
    using JetBrains.ProjectModel;
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.Psi.Util;
    using Runner.Utility;
    using Shims;

    public abstract class FieldElement : Element
    {
        readonly string _fieldName;

        protected FieldElement(MSpecUnitTestProvider provider,
                               IPsi psiModuleManager,
                               ICache cacheManager,
                               Element parent,
                               ProjectModelElementEnvoy projectEnvoy,
                               IClrTypeName declaringTypeName,
                               string fieldName,
                               bool isIgnored)
            : base(provider, psiModuleManager, cacheManager, parent, projectEnvoy, declaringTypeName, isIgnored || parent.Explicit)
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