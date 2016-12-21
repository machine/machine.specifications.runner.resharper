
using JetBrains.ReSharper.UnitTestFramework.Elements;

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
                                           IUnitTestElementManager elementManager,
                                           string fieldName,
                                           bool isIgnored)
            : base(provider,
                   context,
                   declaringTypeName,
                   cachingService,
                   elementManager,
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
                if (this.Context == null)
                {
                    return UnitTestElementCategory.Uncategorized;
                }

                return this.Context.Categories;
            }
        }

        public override UnitTestElementId Id
        {
            get { return this._id; }
        }

        public static UnitTestElementId CreateId(IUnitTestElementIdFactory elementIdFactory, IUnitTestElementsObserver consumer, IUnitTestProvider provider, ContextElement contextElement, string fieldName)
        {
            var result = new[] { contextElement.Id, fieldName };
            var id = result.Where(s => !string.IsNullOrEmpty(s)).AggregateString(".");
            return elementIdFactory.Create(provider, contextElement.GetProject(), consumer.TargetFrameworkId, id);
        }
    }
}
