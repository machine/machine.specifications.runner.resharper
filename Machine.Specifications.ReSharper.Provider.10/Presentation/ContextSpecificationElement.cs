using System.Linq;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;

namespace Machine.Specifications.ReSharperProvider.Presentation
{
    public class ContextSpecificationElement : FieldElement
    {
        public ContextSpecificationElement
            (UnitTestElementId id,
            IUnitTestElement parent,
            IClrTypeName declaringTypeName,
            MspecServiceProvider serviceProvider,
            string fieldName,
            bool isIgnored)
            : base(parent, declaringTypeName, serviceProvider, fieldName, isIgnored || parent.Explicit)
        {
            Id = id;
        }

        public ContextElement Context => (ContextElement)Parent;

        public override string Kind => "Specification";

        public override UnitTestElementId Id { get; }

        public static UnitTestElementId CreateId(IUnitTestElementIdFactory elementIdFactory, IUnitTestElementsObserver consumer, IUnitTestProvider provider, ContextElement contextElement, string fieldName)
        {
            var result = new[] { contextElement.Id, fieldName };
            var id = result.Where(s => !string.IsNullOrEmpty(s)).AggregateString(".");

            return elementIdFactory.Create(provider, contextElement.GetProject(), consumer.TargetFrameworkId, id);
        }
    }
}
