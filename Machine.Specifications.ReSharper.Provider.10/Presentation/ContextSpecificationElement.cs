using System.Collections.Generic;
using System.Linq;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.Util;

namespace Machine.Specifications.ReSharperProvider.Presentation
{
    public class ContextSpecificationElement : FieldElement
    {
        public ContextSpecificationElement(UnitTestElementId id,
                                           ContextElement context,
                                           IClrTypeName declaringTypeName,
                                           UnitTestingCachingService cachingService,
                                           IUnitTestElementManager elementManager,
                                           string fieldName,
                                           bool isIgnored)
            : base(context,
                   declaringTypeName,
                   cachingService,
                   elementManager,
                   fieldName,
                   isIgnored || context.Explicit)
        {
            Id = id;
        }

        public ContextElement Context => (ContextElement)Parent;

        public override string Kind => "Specification";

        public override ISet<UnitTestElementCategory> OwnCategories
        {
            get
            {
                if (Context == null)
                {
                    return UnitTestElementCategory.Uncategorized.ToSet();
                }

                return Context.OwnCategories;
            }
        }

        public override UnitTestElementId Id { get; }

        public static UnitTestElementId CreateId(IUnitTestElementIdFactory elementIdFactory, IUnitTestElementsObserver consumer, IUnitTestProvider provider, ContextElement contextElement, string fieldName)
        {
            var result = new[] { contextElement.Id, fieldName };
            var id = result.Where(s => !string.IsNullOrEmpty(s)).AggregateString(".");

            return elementIdFactory.Create(provider, contextElement.GetProject(), consumer.TargetFrameworkId, id);
        }
    }
}
