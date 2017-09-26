using System.Linq;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;

namespace Machine.Specifications.ReSharperProvider.Presentation
{
    public class BehaviorElement : FieldElement
    {
        public BehaviorElement(
            UnitTestElementId id,
            IUnitTestElement parent,
            IClrTypeName declaringTypeName,
            MspecServiceProvider serviceProvider,
            string fieldName,
            bool isIgnored,
            string fieldType)
            : base(parent, declaringTypeName, serviceProvider, fieldName, isIgnored || parent.Explicit)
        {
            FieldType = fieldType;
            Id = id;
        }

        public ContextElement Context => (ContextElement) Parent;

        public string FieldType { get; }

        public override string Kind => "Behavior";

        public override UnitTestElementId Id { get; }

        protected override string GetTitlePrefix()
        {
            return "behaves like";
        }

        public static UnitTestElementId CreateId(IUnitTestElementIdFactory elementIdFactory, IUnitTestElementsObserver consumer, MspecTestProvider provider, ContextElement contextElement, string fieldType, string fieldName)
        {
            var result = new[] { contextElement.Id, fieldType, fieldName };
            var id = result.Where(s => !string.IsNullOrEmpty(s)).AggregateString(".");

            return elementIdFactory.Create(provider, contextElement.GetProject(), consumer.TargetFrameworkId, id);
        }
    }
}
