namespace Machine.Specifications.ReSharperProvider.Presentation
{
    using JetBrains.Metadata.Reader.API;
    using JetBrains.ReSharper.UnitTestFramework;
    using JetBrains.Util;
    using System.Collections.Generic;
    using System.Linq;

    public class BehaviorSpecificationElement : FieldElement
    {
        readonly UnitTestElementId _id;

        public BehaviorSpecificationElement(MSpecUnitTestProvider provider,
                                            UnitTestElementId id,
                                            BehaviorElement behavior,
                                            IClrTypeName declaringTypeName,
                                            UnitTestingCachingService cachingService,
                                            string fieldName,
                                            bool isIgnored
          )
            : base(provider,
                   behavior,
                   declaringTypeName,
                   cachingService,
                   fieldName,
                   isIgnored || behavior.Explicit)
        {
            this._id = id;
        }

        public BehaviorElement Behavior
        {
            get { return (BehaviorElement)this.Parent; }
        }

        public override string Kind
        {
            get { return "Behavior Specification"; }
        }

        public override IEnumerable<UnitTestElementCategory> Categories
        {
            get
            {
                var parent = this.Parent ?? this.Behavior;
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

        public static UnitTestElementId CreateId(IUnitTestElementIdFactory elementIdFactory, IUnitTestProvider provider, BehaviorElement behaviorElement, string fieldName)
        {
            var result = new[] { behaviorElement.Id, fieldName };
            var id = result.Where(s => !string.IsNullOrEmpty(s)).AggregateString(".");
            return elementIdFactory.Create(provider, behaviorElement.GetProject(), id);
        }
    }
}
