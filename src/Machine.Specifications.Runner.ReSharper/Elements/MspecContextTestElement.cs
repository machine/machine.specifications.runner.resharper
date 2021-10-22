using JetBrains.Annotations;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Runner.ReSharper.Elements
{
    public class MspecContextTestElement : ClrUnitTestElement.FromClass
    {
        [UsedImplicitly]
        public MspecContextTestElement()
        {
        }

        public MspecContextTestElement(IClrTypeName typeName, string? subject, string? explicitReason)
            : base(typeName.FullName, typeName, typeName.ShortName.ToFormat())
        {
            Subject = subject;
            ExplicitReason = explicitReason;
        }

        public override string Kind => "Context";

        public string? Subject { get; }

        public string? ExplicitReason { get; }

        public override string GetPresentation()
        {
            var display = TypeName.ShortName.ToFormat();

            return string.IsNullOrEmpty(Subject)
                ? display
                : $"{Subject}, {display}";
        }
    }
}
