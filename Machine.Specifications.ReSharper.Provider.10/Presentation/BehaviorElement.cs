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
            IClrTypeName typeName,
            MspecServiceProvider serviceProvider,
            string fieldName,
            bool isIgnored,
            string fieldType)
            : base(id, parent, typeName, serviceProvider, fieldName, isIgnored || parent.Explicit)
        {
            FieldType = fieldType;
        }

        public ContextElement Context => (ContextElement) Parent;

        public string FieldType { get; }

        public override string Kind => "Behavior";

        protected override string GetTitlePrefix()
        {
            return "behaves like";
        }

        public static UnitTestElementId CreateId(IUnitTestElementIdFactory elementIdFactory, IUnitTestElementsObserver consumer, MspecTestProvider provider, ContextElement contextElement, string fieldType, string fieldName)
        {
            var result = new[] { contextElement.Id, fieldType, fieldName };
            var id = result.Where(s => !string.IsNullOrEmpty(s)).AggregateString(".");

            return elementIdFactory.Create(provider, contextElement.Id.Project, consumer.TargetFrameworkId, id);
        }
    }
}
