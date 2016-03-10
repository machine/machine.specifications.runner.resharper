using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework.Elements;

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
                                            IUnitTestElementManager elementManager,
                                            string fieldName,
                                            bool isIgnored
          )
            : base(provider,
                   behavior,
                   declaringTypeName,
                   cachingService,
                   elementManager,
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
                if (this.Behavior == null)
                {
                    return UnitTestElementCategory.Uncategorized;
                }

                return this.Behavior.Categories;
            }
        }

        public override UnitTestElementId Id
        {
            get { return this._id; }
        }

        public override UnitTestElementDisposition GetDisposition()
        {
            if (this.Behavior == null)
            {
                return UnitTestElementDisposition.InvalidDisposition;
            }

            IDeclaredElement behaviorElement = this.Behavior.GetDeclaredElement();
            if (behaviorElement == null || !behaviorElement.IsValid())
            {
                return UnitTestElementDisposition.InvalidDisposition;
            }

            return base.GetDisposition();
        }

        public override IEnumerable<UnitTestElementLocation> GetLocations(IDeclaredElement element)
        {
            return this.Behavior.GetLocations(element);
        }

        public static UnitTestElementId CreateId(IUnitTestElementIdFactory elementIdFactory, IUnitTestProvider provider, BehaviorElement behaviorElement, string fieldName)
        {
            var result = new[] { behaviorElement.Id, fieldName };
            var id = result.Where(s => !string.IsNullOrEmpty(s)).AggregateString(".");
            return elementIdFactory.Create(provider, behaviorElement.GetProject(), id);
        }
    }
}
