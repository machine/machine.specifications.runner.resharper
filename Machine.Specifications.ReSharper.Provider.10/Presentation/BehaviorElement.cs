using JetBrains.ReSharper.UnitTestFramework.Elements;

namespace Machine.Specifications.ReSharperProvider.Presentation
{
    using JetBrains.Metadata.Reader.API;
    using JetBrains.ReSharper.UnitTestFramework;
    using JetBrains.Util;
    using System.Collections.Generic;
    using System.Linq;

    public class BehaviorElement : FieldElement
    {
        readonly UnitTestElementId _id;

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
            this.FieldType = fieldType;
            this._id = id;
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

        public override string GetTitlePrefix()
        {
            return "behaves like";
        }

        public static UnitTestElementId CreateId(IUnitTestElementIdFactory elementIdFactory, MSpecUnitTestProvider provider, ContextElement contextElement, string fieldType, string fieldName)
        {
            var result = new[] { contextElement.Id, fieldType, fieldName };
            var id = result.Where(s => !string.IsNullOrEmpty(s)).AggregateString(".");
            return elementIdFactory.Create(provider, contextElement.GetProject(), id);
        }
    }
}
