using System;
using System.Linq;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;
using Machine.Specifications.ReSharperRunner;

namespace Machine.Specifications.ReSharperProvider.Elements
{
    public class BehaviorElement : FieldElement, IEquatable<BehaviorElement>
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

        public ContextElement Context => Parent as ContextElement;

        public string FieldType { get; }

        public override string Kind => "Behavior";

        protected override string GetTitlePrefix()
        {
            return "behaves like";
        }

        public bool Equals(BehaviorElement other)
        {
            return other != null &&
                   Equals(Id, other.Id) &&
                   Equals(TypeName, other.TypeName) &&
                   Equals(FieldName, other.FieldName);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BehaviorElement);
        }

        public override int GetHashCode()
        {
            return HashCode
                .Of(Id)
                .And(TypeName?.FullName)
                .And(FieldName);
        }
    }
}
