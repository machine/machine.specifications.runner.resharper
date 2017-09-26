using System.Collections.Generic;
using System.Linq;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;

namespace Machine.Specifications.ReSharperProvider.Presentation
{
    public class BehaviorSpecificationElement : FieldElement
    {
        public BehaviorSpecificationElement(
            UnitTestElementId id,
            IUnitTestElement parent,
            IClrTypeName declaringTypeName,
            MspecServiceProvider serviceProvider,
            string fieldName,
            bool isIgnored)
            : base(parent, declaringTypeName, serviceProvider, fieldName, isIgnored || parent.Explicit)
        {
            Id = id;
        }

        public BehaviorElement Behavior => (BehaviorElement)Parent;

        public override string Kind => "Behavior Specification";

        public override UnitTestElementId Id { get; }

        public override UnitTestElementDisposition GetDisposition()
        {
            IDeclaredElement behaviorElement = Behavior?.GetDeclaredElement();

            if (behaviorElement == null || !behaviorElement.IsValid())
                return UnitTestElementDisposition.InvalidDisposition;

            return base.GetDisposition();
        }

        public override IEnumerable<UnitTestElementLocation> GetLocations(IDeclaredElement element)
        {
            return Behavior.GetLocations(element);
        }

        public static UnitTestElementId CreateId(IUnitTestElementIdFactory elementIdFactory, IUnitTestElementsObserver consumer, IUnitTestProvider provider, BehaviorElement behaviorElement, string fieldName)
        {
            var result = new[] { behaviorElement.Id, fieldName };
            var id = result.Where(s => !string.IsNullOrEmpty(s)).AggregateString(".");

            return elementIdFactory.Create(provider, behaviorElement.GetProject(), consumer.TargetFrameworkId, id);
        }
    }
}
