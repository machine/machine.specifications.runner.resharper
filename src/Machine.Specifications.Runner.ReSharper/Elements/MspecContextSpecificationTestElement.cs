using System;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.TestRunner.Abstractions.Objects;
using JetBrains.ReSharper.UnitTestFramework;

namespace Machine.Specifications.Runner.ReSharper.Elements
{
    public class MspecContextSpecificationTestElement : MspecFieldTestElement
    {
        public MspecContextSpecificationTestElement(MspecServiceProvider services, UnitTestElementId id, IClrTypeName typeName, string fieldName, string? explicitReason)
            : base(services, id, typeName, fieldName, explicitReason)
        {
        }

        public MspecContextTestElement? Context => Parent as MspecContextTestElement;

        public override string Kind => "Specification";
    }
}
