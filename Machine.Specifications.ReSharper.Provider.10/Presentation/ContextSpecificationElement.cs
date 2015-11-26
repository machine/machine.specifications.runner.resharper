
namespace Machine.Specifications.ReSharperProvider.Presentation
{
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.ProjectModel;
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.UnitTestFramework;
    using JetBrains.Metadata.Reader.API;
    using JetBrains.Util;
    using Machine.Specifications.ReSharperProvider.Shims;

    public class ContextSpecificationElement : FieldElement
    {
        readonly UnitTestElementId _id;

        public ContextSpecificationElement(MSpecUnitTestProvider provider,
                                           IPsi psiModuleManager,
                                           ICache cacheManager,
                                           UnitTestElementId id,
                                           ProjectModelElementEnvoy project,
                                           ContextElement context,
                                           IClrTypeName declaringTypeName,
                                           string fieldName,
                                           bool isIgnored)
            : base(provider,
                   psiModuleManager,
                   cacheManager,
                   context,
                   project,
                   declaringTypeName,
                   fieldName,
                   isIgnored || context.Explicit)
        {
            this._id = id;
        }

        public ContextElement Context
        {
            get { return (ContextElement)this.Parent; }
        }

        public override string Kind
        {
            get { return "Specification"; }
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

        public static UnitTestElementId CreateId(IUnitTestElementIdFactory elementIdFactory, IUnitTestProvider provider, ContextElement contextElement, string fieldName)
        {
            var result = new[] { contextElement.Id, fieldName };
            var id = result.Where(s => !string.IsNullOrEmpty(s)).AggregateString(".");
            return elementIdFactory.Create(provider, contextElement.GetProject(), id);
        }
    }
}
