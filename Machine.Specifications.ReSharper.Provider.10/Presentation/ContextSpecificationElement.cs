
namespace Machine.Specifications.ReSharperProvider.Presentation
{
    using JetBrains.Metadata.Reader.API;
    using JetBrains.ReSharper.UnitTestFramework;
    using JetBrains.Util;
    using System.Collections.Generic;
    using System.Linq;

    public class ContextSpecificationElement : FieldElement
    {
        readonly UnitTestElementId _id;

        public ContextSpecificationElement(MSpecUnitTestProvider provider,
                                           UnitTestElementId id,
                                           ContextElement context,
                                           IClrTypeName declaringTypeName,
                                           UnitTestingCachingService cachingService,
                                           string fieldName,
                                           bool isIgnored)
            : base(provider,
                   context,
                   declaringTypeName,
                   cachingService,
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
