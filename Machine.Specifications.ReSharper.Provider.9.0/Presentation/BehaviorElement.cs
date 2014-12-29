namespace Machine.Specifications.ReSharperProvider.Presentation
{
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Metadata.Reader.API;
    using JetBrains.ProjectModel;
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.UnitTestFramework;
    using JetBrains.Util;
    using Machine.Specifications.ReSharperProvider.Shims;

    public class BehaviorElement : FieldElement
    {
        readonly UnitTestElementId _id;

        public BehaviorElement(MSpecUnitTestProvider provider,
                               IPsi psiModuleManager,
                               ICache cacheManager,
            // ReSharper disable SuggestBaseTypeForParameter
                               ContextElement context,
            // ReSharper restore SuggestBaseTypeForParameter
                               ProjectModelElementEnvoy projectEnvoy,
                               IClrTypeName declaringTypeName,
                               string fieldName,
                               bool isIgnored,
                               string fieldType)
            : base(provider,
                   psiModuleManager,
                   cacheManager,
                   context,
                   projectEnvoy,
                   declaringTypeName,
                   fieldName,
                   isIgnored || context.Explicit)
        {
            this.FieldType = fieldType;
            this._id = CreateId(provider, context, fieldType, fieldName);
        }

        public ContextElement Context
        {
            get { return (ContextElement)this.Parent; }
        }

        public string FieldType { get; private set; }

        public override string Kind
        {
            get { return "Behavior"; }
        }

        public override IEnumerable<UnitTestElementCategory> Categories
        {
            get
            {
                var parent = this.Parent ?? this.Context;
                if (parent == null)
                {
                    return UnitTestElementCategory.Uncategorized;
                }

                return parent.Categories;
            }
        }

        public override UnitTestElementId Id
        {
            get { return this._id; }
        }

        public override string GetTitlePrefix()
        {
            return "behaves like";
        }

        public static UnitTestElementId CreateId(MSpecUnitTestProvider provider, ContextElement contextElement, string fieldType, string fieldName)
        {
            var result = new[] { contextElement.Id, fieldType, fieldName };
            var id = result.Where(s => !string.IsNullOrEmpty(s)).AggregateString(".");
            return new UnitTestElementId(provider, new PersistentProjectId(contextElement.GetProject()), id);
        }
    }
}
