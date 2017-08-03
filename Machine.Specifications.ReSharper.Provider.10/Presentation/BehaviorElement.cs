using System.Collections.Generic;
using System.Linq;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.Util;

namespace Machine.Specifications.ReSharperProvider.Presentation
{
    public class BehaviorElement : FieldElement
    {
        public BehaviorElement(MSpecUnitTestProvider provider,
                               UnitTestElementId id,
                               ContextElement context,
                               IClrTypeName declaringTypeName,
                               UnitTestingCachingService cachingService,
                               IUnitTestElementManager elementManager,
                               string fieldName,
                               bool isIgnored,
                               string fieldType)
            : base(provider,
                   context,
                   declaringTypeName,
                   cachingService,
                   elementManager,
                   fieldName,
                   isIgnored || context.Explicit)
        {
            FieldType = fieldType;
            Id = id;
        }

        public ContextElement Context => (ContextElement)Parent;

        public string FieldType { get; }

        public override string Kind => "Behavior";

        public override ISet<UnitTestElementCategory> OwnCategories
        {
            get
            {
                if (Context == null)
                    return UnitTestElementCategory.Uncategorized.ToSet();

                return Context.OwnCategories;
            }
        }

        public override UnitTestElementId Id { get; }

        protected override string GetTitlePrefix()
        {
            return "behaves like";
        }

        public static UnitTestElementId CreateId(IUnitTestElementIdFactory elementIdFactory, IUnitTestElementsObserver consumer, MSpecUnitTestProvider provider, ContextElement contextElement, string fieldType, string fieldName)
        {
            var result = new[] { contextElement.Id, fieldType, fieldName };
            var id = result.Where(s => !string.IsNullOrEmpty(s)).AggregateString(".");

            return elementIdFactory.Create(provider, contextElement.GetProject(), consumer.TargetFrameworkId, id);
        }
    }
}
